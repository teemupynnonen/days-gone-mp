using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace DaysGoneMP
{
    public class NetworkManager : MonoBehaviour
    {
        public static NetworkManager Instance { get; private set; }

        [Header("Network Settings")]
        public int Port = 7777;
        public string ServerAddress = "127.0.0.1";

        private UdpClient _udpClient;
        private Thread _receiveThread;
        private bool _isRunning;
        private bool _isHost;
        private bool _isClient;

        // Mission State Synchronization
        private Dictionary<int, MissionState> _activeMissions = new Dictionary<int, MissionState>();
        private object _missionLock = new object();

        public event Action<MissionState> OnMissionUpdated;
        public event Action<MissionState> OnMissionCompleted;
        public event Action OnConnectionEstablished;
        public event Action OnConnectionLost;

        public bool IsHost => _isHost;
        public bool IsClient => _isClient;
        public bool IsConnected => _isHost || _isClient;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void StartHost()
        {
            if (_isHost) return;

            _isHost = true;
            _isClient = false;
            InitializeUdpClient();
            StartReceiveThread();
            Debug.Log($"[Network] Host started on port {Port}");
        }

        public void StartClient(string serverAddress)
        {
            if (_isClient) return;

            _isClient = true;
            _isHost = false;
            ServerAddress = serverAddress;
            InitializeUdpClient();
            StartReceiveThread();
            SendHandshake();
            Debug.Log($"[Network] Client started, connecting to {ServerAddress}:{Port}");
        }

        private void InitializeUdpClient()
        {
            _udpClient = new UdpClient();
            if (_isHost)
            {
                _udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, Port));
            }
            else
            {
                _udpClient.Client.Connect(ServerAddress, Port);
            }
        }

        private void StartReceiveThread()
        {
            _isRunning = true;
            _receiveThread = new Thread(ReceiveLoop);
            _receiveThread.IsBackground = true;
            _receiveThread.Start();
        }

        private void ReceiveLoop()
        {
            while (_isRunning)
            {
                try
                {
                    IPEndPoint remoteEndPoint = null;
                    byte[] data = _udpClient.Receive(ref remoteEndPoint);
                    if (data != null && data.Length > 0)
                    {
                        ProcessPacket(data);
                    }
                }
                catch (Exception e)
                {
                    if (_isRunning)
                    {
                        Debug.LogError($"[Network] Receive error: {e.Message}");
                    }
                }
            }
        }

        private void ProcessPacket(byte[] data)
        {
            // Simple packet structure: [1 byte type][payload]
            if (data.Length < 1) return;

            byte packetType = data[0];
            byte[] payload = new byte[data.Length - 1];
            Array.Copy(data, 1, payload, 0, payload.Length);

            switch (packetType)
            {
                case (byte)PacketType.Handshake:
                    HandleHandshake(payload);
                    break;
                case (byte)PacketType.MissionUpdate:
                    HandleMissionUpdate(payload);
                    break;
                case (byte)PacketType.MissionComplete:
                    HandleMissionComplete(payload);
                    break;
                case (byte)PacketType.MissionStateSync:
                    HandleMissionStateSync(payload);
                    break;
            }
        }

        private void HandleHandshake(byte[] payload)
        {
            if (_isClient)
            {
                OnConnectionEstablished?.Invoke();
                // Request full mission state sync from host
                SendMissionStateRequest();
            }
        }

        private void HandleMissionUpdate(byte[] payload)
        {
            MissionState state = DeserializeMissionState(payload);
            if (state != null)
            {
                lock (_missionLock)
                {
                    _activeMissions[state.MissionId] = state;
                }
                OnMissionUpdated?.Invoke(state);
            }
        }

        private void HandleMissionComplete(byte[] payload)
        {
            int missionId = BitConverter.ToInt32(payload, 0);
            lock (_missionLock)
            {
                if (_activeMissions.TryGetValue(missionId, out MissionState state))
                {
                    state.IsCompleted = true;
                    state.CompletionTime = DateTime.UtcNow;
                    OnMissionCompleted?.Invoke(state);
                }
            }
        }

        private void HandleMissionStateSync(byte[] payload)
        {
            // Full sync of all active missions
            int count = BitConverter.ToInt32(payload, 0);
            int offset = 4;
            for (int i = 0; i < count; i++)
            {
                int stateSize = BitConverter.ToInt32(payload, offset);
                offset += 4;
                byte[] stateData = new byte[stateSize];
                Array.Copy(payload, offset, stateData, 0, stateSize);
                offset += stateSize;

                MissionState state = DeserializeMissionState(stateData);
                if (state != null)
                {
                    lock (_missionLock)
                    {
                        _activeMissions[state.MissionId] = state;
                    }
                    OnMissionUpdated?.Invoke(state);
                }
            }
        }

        public void SendHandshake()
        {
            byte[] packet = new byte[1];
            packet[0] = (byte)PacketType.Handshake;
            SendPacket(packet);
        }

        public void SendMissionUpdate(MissionState state)
        {
            byte[] payload = SerializeMissionState(state);
            byte[] packet = new byte[1 + payload.Length];
            packet[0] = (byte)PacketType.MissionUpdate;
            Array.Copy(payload, 0, packet, 1, payload.Length);
            SendPacket(packet);
        }

        public void SendMissionComplete(int missionId)
        {
            byte[] payload = BitConverter.GetBytes(missionId);
            byte[] packet = new byte[1 + payload.Length];
            packet[0] = (byte)PacketType.MissionComplete;
            Array.Copy(payload, 0, packet, 1, payload.Length);
            SendPacket(packet);
        }

        public void SendMissionStateRequest()
        {
            byte[] packet = new byte[1];
            packet[0] = (byte)PacketType.MissionStateSync;
            SendPacket(packet);
        }

        public void SendFullMissionStateSync()
        {
            lock (_missionLock)
            {
                if (_activeMissions.Count == 0) return;

                List<byte[]> serializedStates = new List<byte[]>();
                int totalSize = 4; // Count
                foreach (var kvp in _activeMissions)
                {
                    byte[] stateData = SerializeMissionState(kvp.Value);
                    serializedStates.Add(stateData);
                    totalSize += 4 + stateData.Length; // Size + Data
                }

                byte[] payload = new byte[totalSize];
                int offset = 0;
                byte[] countBytes = BitConverter.GetBytes(_activeMissions.Count);
                Array.Copy(countBytes, 0, payload, offset, 4);
                offset += 4;

                foreach (byte[] stateData in serializedStates)
                {
                    byte[] sizeBytes = BitConverter.GetBytes(stateData.Length);
                    Array.Copy(sizeBytes, 0, payload, offset, 4);
                    offset += 4;
                    Array.Copy(stateData, 0, payload, offset, stateData.Length);
                    offset += stateData.Length;
                }

                byte[] packet = new byte[1 + payload.Length];
                packet[0] = (byte)PacketType.MissionStateSync;
                Array.Copy(payload, 0, packet, 1, payload.Length);
                SendPacket(packet);
            }
        }

        private void SendPacket(byte[] packet)
        {
            if (_udpClient == null || !_isRunning) return;
            try
            {
                if (_isHost)
                {
                    // In a real implementation, we'd track connected clients
                    // For now, broadcast to all known clients
                    // This is a simplified version
                }
                else
                {
                    _udpClient.Send(packet, packet.Length);
                }
            }
            catch (Exception e)
            {

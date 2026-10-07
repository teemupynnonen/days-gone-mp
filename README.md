<img src="assets/icon.svg" width="64" height="64" alt="Days Gone MP">

# Days Gone MP

An experimental co-op mod for **Days Gone on PC**, supporting **up to four players: one host and three guests**.

**The ultimate goal is to let you play through Days Gone's entire story with your friends.**

**This is very much a work in progress.** There are known issues with game behavior not working correctly for guests, and missions may not work or progress for guest players. These issues are being addressed over time.

Development priorities, in order:

1. **Stabilize the core co-op functionality** so the multiplayer foundation works reliably.
2. **Work through the story mission by mission**, fixing guest behavior and progression along the way.
3. **Polish the smaller details** that are annoying but do not block playing together.

This repository is the home for public builds, release notes, setup instructions, and bug reports. **The mod's source code is not published here yet.**

[Releases](https://github.com/teemupynnonen/days-gone-mp/releases) · [Setup](#setup) · [Play-together guide](#play-together) · [Report a bug](https://github.com/teemupynnonen/days-gone-mp/issues)

## Downloads and status

**[Download v0.3.0-alpha.1](https://github.com/teemupynnonen/days-gone-mp/releases/tag/v0.3.0-alpha.1)** — the current alpha prerelease for Windows x64.

Download **`DaysGoneMP-0.3.0-alpha.1-windows-x64.zip`** from the release's **Assets** section. GitHub's automatically generated **Source code** archives contain this repository's documentation, not the playable mod. Check the release notes for features and known issues. The F9 menu shows your mod version, and each package includes `VERSION.txt`; its release also provides a SHA-256 checksum.

The mod is in active development. Combat, world synchronization, and campaign progression remain experimental. Automated networking tests cover three- and four-player sessions; see the release notes for the checks performed on each published build. Automated coverage does not establish complete four-player gameplay or Steam internet compatibility.

## What the mod can do

- **Explore with friends.** See each other's movement, animations, aiming, weapons, and flashlights. Find teammates through overhead names, map markers, and health bars.
- **Ride together.** Each player's motorcycle is visible, including its equipped parts, paint, finishes, tank decals, riding animations, lights, engine audio, and nitro effects. Parked bikes remain visible after dismounting.
- **Fight in a shared world.** Replication covers supported freakers, hordes, wildlife, and human NPCs, with guest attacks sent to the host's combat simulation. Gunfire, throwables, explosives, traps, and fire have synchronization support.
- **Help your teammates.** Use a bandage or medkit on a nearby injured player, or revive a downed teammate through the game's interaction prompts.
- **Share the environment.** Time of day and weather follow the host. Camp gates, searchable containers, and supported world sounds also synchronize.
- **Interact with more of the world.** Experimental synchronization covers supported doors, generators, fuse panels, pushable vehicles, ladders, and other mechanisms. Native gameplay behavior across every supported mechanism still needs testing.
- **Progress through common activities.** Experimental progression shares objectives for missions, side jobs, and encounters available to the participating players. Each player keeps their own campaign and inventory; see [Saves and current limits](#saves-and-current-limits).
- **Choose how to connect.** Direct IP supports LAN, same-PC sessions, and reachable internet hosts. The experimental Steam integration adds friends-only lobbies, joining directly from the friends list, join codes, and relay networking.
- **Choose your appearance and view.** Select a character model in the F9 menu or press **F10** for the optional first-person camera. Riding in first person is a separate option. Each client remembers its character and camera preferences.

## Requirements

- **Windows x64** and an installed copy of **Days Gone on Steam**.
- **Steam game build 19221447.** The launcher checks the executable and rejects unsupported versions. Other storefronts and game builds are not currently supported.
- The **same mod release on every player’s PC**.
- A loaded save in the same game world for each player. Start in the same open area with compatible campaign progress.
- Steam installed and signed in for the launcher's normal launch flow. Steam co-op requires separate accounts; multiple windows on one account use Direct IP.

The download contains the launcher and mod, not Days Gone. No developer tools, manual DLL installation, or administrator access are required to use the launcher.

## Setup

1. Close Days Gone. Back up the saves you intend to use: experimental progression can advance objectives and write checkpoints in each player's own campaign. The ordinary game's save data is under `%LOCALAPPDATA%\BendGame\Saved`.
2. Extract the entire release ZIP, then open the extracted **DaysGoneMP** folder. Keep **DaysGoneMPLauncher.exe**, **DaysGoneMP.dll**, **launcher_update.ps1**, and the **steam** folder together. Run the extracted files, not the files inside the ZIP.
3. Start Steam and sign in, then double-click **DaysGoneMPLauncher.exe**.
4. Click **Play with Steam**. If the launcher cannot find the game, click **Game location...** and select `BendGame\Binaries\Win64\DaysGone.exe` inside your Days Gone installation.
5. Keep the launcher open until it reports **mod enabled**. Load a save and enter gameplay.
6. Press **F9**, open **Co-op**, and choose a connection method below. Every player needs to complete these setup steps; the launcher does not host or join automatically.

The launcher loads the mod into the running game without replacing game installation files.

## Play together

### Direct IP

Use this for a local network or multiple windows on one PC. Internet play also works when the host's address and UDP port are reachable.

1. Everyone opens **F9 → Co-op → Direct IP**.
2. The host chooses a **Port** (default **27800**) and clicks **Host without Steam**.
3. Each guest enters the host's IPv4 address in **Host IP**, enters the same port in the separate **Port** field, and clicks **Join by IP**.
4. Wait for the connection to complete, then close the menu with **F9**. Every guest joins the same host; a session has room for three guests.

Choose the address for your setup:

- **Same PC:** `127.0.0.1`.
- **Same local network:** the host PC's LAN IPv4 address.
- **Internet:** the host's public IPv4 address. The host must forward the chosen **UDP** port to their PC and allow it through the firewall. Direct IP has no automatic relay or router configuration. Carrier-grade NAT can prevent incoming connections.

Enter an IPv4 address, not a hostname or IPv6 address. Direct IP does not use Steam networking, but it still requires the supported Steam edition of the game.

### Steam friends

The current Steam integration uses **Spacewar (AppID 480)** as development infrastructure. Steam may display that name while the companion is running. Steam co-op and internet relay play are experimental; use the release notes to check their verification status.

1. Everyone starts the mod, loads a save, and selects **F9 → Co-op → Steam**. Use separate Steam accounts and be friends with the host.
2. The host clicks **Host co-op** and waits for **Session ready**.
3. Each guest finds the host in the **Friends online** list and clicks **Join** beside their name. Click **Refresh** if the host's session has not appeared yet. Up to three guests can join.
4. Alternatively, the host clicks **Copy code** and shares it. Guests paste it into the join-code field and click **Join with code**. A code still requires access to the friends-only lobby.

All players must have the mod running and a save loaded before joining. Steam mode uses relay networking, so the Direct IP port-forwarding steps do not apply.

### Multiple windows on one PC

1. Use **Play with Steam** for the first window.
2. Click **Launch another client** for each additional window. Load a save in each game.
3. Host through **Direct IP** in one window, then join `127.0.0.1` and the host's port from each guest.

Additional clients get separate saves, settings, and logs. On first launch, a client copies the original save profile; later launches preserve that client's own progress. Its profile is stored at `%LOCALAPPDATA%\BendGame\Saved_DGMP_clientN`, where `N` is the client number shown by the launcher. If multiple Steam save profiles make the initial copy ambiguous, the launcher asks you to set that client up first.

The launcher can open more windows than one session can admit, subject to your PC's resources. A single session always has a maximum of four players, and a fifth player is rejected as full.

## During a session

- **F9** opens or closes the mod menu. **Co-op → Show teammate health** controls the teammate health display.
- **F9 → Character & diagnostics → Summon bike** brings your assigned bike three metres in front of you. Dismount and face open ground first; your bike must already be loaded in the current world.
- **F10** toggles the experimental first-person camera. Under **F9 → Character & diagnostics**, enable **Enable First Person View for riding** to keep it on while riding; it defaults to off until you enable it. Scripted cameras retain their normal view.
- Your character model, first-person toggle, riding toggle, and first-person FOV are saved automatically for each client and restored next time you play. Choose **Story appearance** in the character selector to clear the model override.
- To **heal**, approach an injured teammate on foot within 2.5 metres and with a clear line of sight. Hold the displayed bandage or medkit interaction for two seconds. It consumes one selected item from the healer's inventory.
- To **revive**, approach a downed teammate and hold **REVIVE PLAYER** for ten seconds. A successful revive restores 10% maximum health without consuming an item. Players normally have a 60-second downed window; an active revive pauses that countdown. Releasing the button or moving out of reach cancels the hold.
- **Opening the pause menu does not pause the world while connected.** Stay somewhere safe when using menus.
- Established sessions retry after an unexpected connection loss. Use **Co-op → Cancel reconnect** to stop retrying.
- Use **Co-op → Leave session** or **F8** to disconnect. A guest leaving frees a slot while the remaining players continue. The host leaving ends the session; host migration is not available.

## Saves and current limits

Each player runs their own game and uses their own saves and inventory. Co-op does not replace a guest's campaign with the host's save.

Progression sharing works per activity: eligible guests can catch up on objectives and checkpoints for content they have unlocked. Guests with missing prerequisites, already completed content, or progress ahead of the host can help without having their campaign rolled back. Locked story content is not imported. Some scripted interactions still need the host to perform them, and full campaign compatibility is not established.

The host controls shared enemies and the environment. Stay near the host: supported enemies in a distant guest's area may be absent if the host has not loaded that area. Supported enemy families are replicated, but this is not complete synchronization of every NPC, script, or object in the game. Combat and AI targeting can still have rough edges. Each player controls their own bike; shared vehicle ownership and collision forces are not synchronized.

The first-person view uses the existing character animations, so some outfits, poses, and close walls can cause clipping. Other mods and unsupported game versions have not been validated with this mod.

## Updating or playing without the mod

The launcher checks GitHub for updates on startup; **Check for updates** checks again. When a newer release is available, choose **Update and restart** to download and verify the package, update the launcher, mod and Steam companion, and reopen the launcher. Saves and settings are preserved. Restart running games afterward to load the new mod, and update all players together. Alpha builds also receive newer prereleases.

For older launchers without update buttons, close Days Gone, extract the new release into a fresh folder, and launch from that folder. Keep the entire package together, including `launcher_update.ps1` in releases that support automatic updates.

To play without the mod, close the game and launch it normally from Steam. Closing the launcher alone leaves the running game and loaded mod active. The mod's extracted folder can be removed once the game is closed; save profiles and logs remain in their local application-data folders.

## Troubleshooting

- **No F9 menu:** enter a loaded gameplay scene and check that the launcher reports **mod enabled**. If startup failed, use **Open logs** in the launcher.
- **Unsupported game:** this mod accepts only Steam build **19221447**. Check the release notes for any updated compatibility information.
- **Missing files or startup errors:** extract the complete ZIP to a fresh folder. Use **Game location...** if automatic game detection failed.
- **Steam unavailable:** finish signing in or updating Steam, then choose **Retry Steam** in the Co-op menu. Use Direct IP for multiple windows on one account.
- **Cannot connect:** check that everyone uses the same mod release, that the host has started a session, and that a guest slot is free. For Direct IP, check the address, matching UDP port, firewall, and router forwarding where applicable.
- **Connected but cannot see a teammate:** load saves in the same game world and travel to the same area. Make sure every game shows a completed connection.
- **Session full:** four players, including the host, is the cap. A guest must leave before another player can join.

Logs are under **`%LOCALAPPDATA%\DaysGoneMP`**. The launcher’s **Open logs** shortcut opens this location. `launcher.log` records startup; `main`, `client2`, `client3`, and other client folders contain their own `local-demo.log` files.

## Bug reports

[Open an issue](https://github.com/teemupynnonen/days-gone-mp/issues/new) with the mod release, game build, connection method, player count, and whether you were the host or a guest. Include steps to reproduce, what you expected, what happened, and relevant log excerpts. Check logs for personal paths, account details, or network addresses before posting them publicly.

For maintainers, [RELEASING.md](https://github.com/teemupynnonen/days-gone-mp/blob/main/RELEASING.md) describes how to publish a packaged build.

Days Gone MP is an unofficial community mod and is not affiliated with or endorsed by Bend Studio or Sony Interactive Entertainment.

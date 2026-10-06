# Publishing a build

This repository contains public documentation and release metadata. Build the mod in the development checkout, then upload the packaged ZIP as a GitHub Release asset. Keep development source, game files, saves, logs, and debug symbols out of this repository and its release assets.

## Prepare and verify

1. Set `VERSION` in the development checkout to the next release number and copy that number into this public repository's `VERSION`. Start with `0.1.0-alpha.1`, increment the alpha number for subsequent alpha builds, and use `beta`, `rc`, or a stable version when appropriate. Published versions identify fixed packages; never replace one with different binaries.

2. Update `docs/PLAYER-GUIDE.md` and `docs/QUICKSTART.txt` in the development checkout, then copy `docs/PLAYER-GUIDE.md` to this repository's `README.md`. Build the package:

   ```powershell
   .\scripts\package.ps1 -Configuration RelWithDebInfo
   ```

   This reads `VERSION` and creates `build\DaysGoneMP-<version>-windows-x64.zip` plus a `.zip.sha256` checksum file. The ZIP contains a `DaysGoneMP` folder with `VERSION.txt`, the launcher, mod DLL, Steam companion/runtime, user guides, and dependency license notices.

3. Run the automated suite against that build from a developer shell:

   ```powershell
   .\scripts\enter-dev-shell.ps1
   ctest --test-dir build/windows-relwithdebinfo --output-on-failure
   ```

4. Check the archive contents and test launching from a freshly extracted copy. The package should contain only the intended runtime files, guides, and notices. Read the packaged guides too: they must agree with this repository's setup instructions.
5. Record the game build, mod version, features changed, known issues, and actual playtest coverage. Exercise a host with at least two guests. For admission or lifecycle changes, also check a full four-player session, rejection of a fifth player, and reuse of a departing guest's slot. Distinguish automated coverage from native gameplay and Steam internet tests.
6. Describe the Steam integration shipped in the release. The first alpha uses Spacewar/AppID 480 as development infrastructure. Preserve the third-party notices included with the package.
7. Update the public README's download link and asset filename, commit the documentation and `VERSION` in this repository, and push before creating the release tag.

## Create a draft release

Use a versioned tag, such as `v0.1.0-alpha.1`, and mark experimental builds as prereleases. The tag in this repository identifies the public documentation and release; keep the exact development revision and build provenance in the development checkout.

Create a release on [GitHub Releases](https://github.com/teemupynnonen/days-gone-mp/releases), or use the GitHub CLI from the development checkout. For example, after preparing release notes at `build\release-notes.md`:

```powershell
gh release create v0.1.0-alpha.1 build/DaysGoneMP-0.1.0-alpha.1-windows-x64.zip build/DaysGoneMP-0.1.0-alpha.1-windows-x64.zip.sha256 --repo teemupynnonen/days-gone-mp --target main --draft --prerelease --title "Days Gone MP v0.1.0-alpha.1" --notes-file build/release-notes.md
```

Include the following in the release notes:

- Supported Steam game build and the requirement for all players to use the same mod release.
- Changes since the previous release, or the initial feature set.
- Known issues and any limitations that affect setup, saves, or multiplayer.
- Which gameplay and connection scenarios were actually tested.
- A link to the [setup guide](https://github.com/teemupynnonen/days-gone-mp#setup).

Upload the versioned ZIP and its checksum under **Assets**. Binaries belong in release assets, not in Git commits. GitHub also adds source archives automatically; those contain only this public repository's files.

Review the draft and its attached ZIP, then publish it when ready. Open the public release page and download the asset to verify that it is the intended package. If a published build needs a fix, create a new version so players can identify exactly which build they have.

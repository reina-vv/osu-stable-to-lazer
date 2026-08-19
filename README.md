# stable-to-lazer importer

A Windows command-line tool that imports one osu!stable beatmap-set folder into
an existing osu!lazer data directory. It reuses lazer's beatmap parser, Realm
model, and file store, and requests NTFS hard links for imported files.

> Unofficial project. It is not affiliated with, endorsed by, or supported by
> ppy Pty Ltd or osu!.

## Usage

Close osu!lazer completely, back up its data directory, then run:

```powershell
.\osu-stable-to-lazer.exe "C:\Users\<username>\AppData\Local\osu!\Songs\123 Artist - Title"
```

If automatic data-directory discovery is unsuitable, specify it explicitly:

```powershell
.\osu-stable-to-lazer.exe "C:\Users\<username>\AppData\Local\osu!\Songs\123 Artist - Title" --lazer-data "C:\Users\<username>\AppData\Roaming\osu"
```

The source and lazer data directories must be on the same writable NTFS volume.
The tool refuses to run while the release or Debug lazer IPC endpoint is active.

## Build

```powershell
dotnet build -c Release -warnaserror
```

The dependency versions are deliberately pinned. Update all `ppy.osu.Game*`
packages together and validate against the corresponding lazer release before
using a new version with real user data.

## Distribution

This repository distributes source code only. It does not distribute compiled
executables, `dotnet publish` output, or restored NuGet runtime packages. Build
the tool locally; NuGet retrieves its dependencies during restore.

Do not redistribute compiled output or use it commercially without independently
reviewing every dependency's licence terms. In particular, lazer game resources
have separate non-commercial and font licence requirements; see the third-party
notices below.

## Licensing and trademarks

The project-authored source code is licensed under the [MIT License](LICENSE).
That licence does not grant rights to third-party packages or assets. See
[third-party notices](THIRD_PARTY_NOTICES.md) for osu!lazer attribution and
dependency obligations. `osu!`, `osu`, `lazer`, and ppy branding belong to ppy
Pty Ltd; this repository is not an official project.

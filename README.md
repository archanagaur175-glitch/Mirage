# Mirage

Turn **Windows 11** into **macOS Tahoe** — a desktop transformation app with a
fluid Dock, a top status HUD, macOS-style traffic lights, a one-click theme &
asset wizard, and an instant, safe Revert Switch.

> Built with C# / .NET 8 and **WinUI 3 (Windows App SDK)**. All shell
> modifications are additive and reversible via documented public Win32 / WinRT
> / DWM APIs only. **Explorer.exe is never killed, terminated, or patched**, and
> **no code is injected into third-party processes**.

## Features

| Feature | What it does |
|---|---|
| **Fluid Dock** | macOS-style magnification dock. Reserves the bottom edge as a *normal* appbar while the real taskbar is set to auto-hide underneath — no edge conflict, no fighting. |
| **Top Status HUD** | Anchored topmost layered window with a Mica backdrop: clock, battery (WinRT), and Wi-Fi / Bluetooth quick toggles. |
| **Traffic Lights** | macOS red/yellow/green buttons on Mirage-owned windows (custom title bar) **and** an out-of-process overlay for third-party apps that occludes the native caption buttons and forwards clicks. Never in-process injection. |
| **Theme & Asset Wizard** | Applies license-safe Tahoe-inspired wallpapers (generated at runtime), system fonts, and cursor schemes. **No Apple proprietary assets are redistributed.** |
| **Instant Safe Revert** | One click replays every recorded system mutation in reverse via the same documented APIs. Zero orphaned registry keys, hooks, or autostart entries. |

## Trust: zero telemetry

Mirage makes **no network requests** of any kind. There is no analytics SDK, no
crash reporter that phones home, and **no `HttpClient` / `System.Net` / socket
usage anywhere in the runtime app** (enforced by a CI grep gate and a unit
test). Crash reporters, update checkers, and telemetry are deliberately absent.

## Requirements

- Windows 11 **22H2 or later** (for Mica / system-backdrop APIs).
- .NET 8 runtime (the published build is self-contained, but the Windows App SDK
  runtime is bundled automatically).

## Build & run (developers)

All compilation happens in GitHub Actions — there are no required local builds.
To run the app, download the latest self-contained EXE from the
[Releases](https://github.com/archanagaur175-glitch/Mirage/releases) page, or
clone and build with:

```powershell
dotnet build src/Mirage.App/Mirage.App.csproj -c Release
dotnet publish src/Mirage.App/Mirage.App.csproj -c Release -r win-x64 -p:SelfContained=true -p:WindowsAppSDKSelfContained=true -o out
```

### SmartScreen note

The distributed build is **unsigned** (no EV code-signing certificate is
available in this open-source project). Windows SmartScreen may warn on first
launch. This is expected for an unsigned open-source binary; verify the checksum
from the release notes if concerned.

## Revert / uninstall

Open Mirage → **Instant Safe Revert**. This closes all Mirage surfaces and
replays the state manifest in reverse, restoring your original wallpaper, taskbar
state, and any other mutations. You can then delete the Mirage folder.

## License

[MIT](LICENSE).

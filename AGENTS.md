# AGENTS.md — Project conventions for agent sessions

This file records the conventions future agents (and humans) must follow when
working in the Mirage repository.

## Repository shape

```
src/
  Mirage.Native/   All P/Invoke + Win32/WinRT interop. Single auditable home.
  Mirage.Core/     App-agnostic logic: StateManifest, FeatureManager, services, models, physics.
  Mirage.App/      WinUI 3 (Windows App SDK) shell: windows, controls, overlay.
  Mirage.Tests/    Headless xUnit tests only (no UI automation).
.github/workflows/ build.yml (validation) + release.yml (tag/manual publish).
```

## Hard constraints (do not violate)

1. **Never** kill, terminate, or binary-patch `explorer.exe` or any Windows
   system file. All shell changes are additive and reversible via documented
   public APIs only.
2. **Never** inject code into third-party process address spaces (no
   `CreateRemoteThread`, DLL injection, or in-process non-client mutation of
   foreign windows). Use the out-of-process overlay-window technique.
3. **Zero** external network requests, analytics, or telemetry in the shipped
   runtime app. The CI `build.yml` **greps** `src/Mirage.App` for forbidden
   networking/telemetry tokens and fails the build on any match. The unit test
   `TelemetryGuardTests` enforces the same offline. If you add a network path,
   it must be opt-in, disabled by default, and isolated — but for this build
   there is intentionally none.
4. **No local builds.** Validation is exclusively via GitHub Actions. Do not run
   `dotnet build`/`dotnet test` locally as a gate; commit and let CI validate.

## Coding conventions

- Language: C# / .NET 8, `net8.0-windows10.0.19041.0` TFM.
- UI: WinUI 3 via `Microsoft.WindowsAppSDK` (version pinned in `Mirage.App.csproj`).
  `WindowsPackageType=None` => unpackaged, self-contained EXE is the v1 default.
- P/Invoke goes **only** in `Mirage.Native`. Wrap raw `DllImport`s in small,
  named, safe static classes. Do not sprinkle `DllImport` across the app.
- `Mirage.Core` must stay framework-agnostic and headless-testable. Keep all
  `Windows.*` WinRT device usage in `Mirage.App`.
- Every system mutation must be recorded in `StateManifest` so the Revert Switch
  can replay it exactly in reverse.
- Prefer documented public APIs (`SHAppBarMessage`, `Dwm*`, `SystemParametersInfo`,
  `SetWinEventHook`) over undocumented/hacky approaches.

## Feature toggles

All five features are independently toggleable via `FeatureManager`. Toggling one
must never touch another's state. The Revert Switch calls `DisableAll()` and then
replays the manifest.

## CI / auto-heal

- `build.yml` runs on every push/PR: restore → build App → build Tests →
  telemetry grep → headless tests → publish self-contained EXE → upload artifact.
- Commit the **entire** change first, push, then trigger CI. Do not trigger CI
  against a partial commit.
- If a run is red, read the failed-job logs (`gh run view --log-failed`), patch
  the offending file, commit, push, and re-trigger. Repeat until green.

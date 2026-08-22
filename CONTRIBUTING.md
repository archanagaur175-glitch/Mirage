# Contributing to Mirage

Thanks for your interest in Mirage! This document covers how to contribute safely
and in line with the project's hard constraints.

## Before you start

- Read [AGENTS.md](AGENTS.md). It enumerates the non-negotiable constraints
  (no explorer.exe patching, no third-party injection, zero telemetry, no local
  builds as a gate).
- All shell modifications must use documented public Win32 / WinRT / DWM APIs and
  must be recorded in `Mirage.Core.StateManifest` so the Revert Switch can undo
  them.
- **No networking.** Do not add `HttpClient`, `System.Net`, sockets, analytics,
  or telemetry SDKs to the runtime app. The CI build fails on any such reference
  (see `TelemetryGuardTests` and the grep step in `build.yml`).

## Assets

- Fonts, cursors, and wallpapers must be **license-safe**. Do **not** redistribute
  Apple's SF Pro or any proprietary Apple artwork. The bundled wallpaper is
  generated at runtime as an original gradient.

## Workflow

1. Make your change in `src/`.
2. Add or update headless tests under `src/Mirage.Tests/` for any logic you touch.
3. Commit the **entire** change and push to `main` (or open a PR).
4. Let GitHub Actions validate. Do not treat local builds as the gate.
5. If CI is red, fix the root cause, commit, push, and re-trigger.

## Code style

- C# / .NET 8, `net8.0-windows10.0.19041.0`.
- Keep P/Invoke isolated in `Mirage.Native`; keep `Mirage.Core` headless.
- Prefer public documented APIs over undocumented hacks.

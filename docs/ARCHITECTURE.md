# Architecture

Mirage is a Windows 11 → macOS Tahoe transformation layer. It is composed of four
projects plus CI:

- **Mirage.Native** — the *single* home for all P/Invoke (SHAppBarMessage,
  DWM, SystemParametersInfo, comctl32 subclassing, user32 window helpers,
  SetWinEventHook). No other project may declare `DllImport`.
- **Mirage.Core** — app-agnostic, headless-testable logic: `StateManifest`,
  `FeatureManager`, the five `*Service` classes, `RevertSwitch`, the spring-physics
  model, and an asset factory.
- **Mirage.App** — the WinUI 3 (Windows App SDK) shell: `MainWindow`, `DockWindow`,
  `HudWindow`, `SettingsWindow`, `SetupWizardWindow`, the traffic-light controls,
  and the out-of-process overlay + controller.
- **Mirage.Tests** — headless xUnit tests only.

## Edge conflict resolution (Dock vs Taskbar)

The Dock reserves the bottom edge as a **normal (non-autohide)** appbar
(`ABM_NEW`/`ABM_SETPOS`). The real Explorer taskbar is simultaneously set to
**`ABS_AUTOHIDE`**. Windows forbids only two *autohide* appbars on one edge, and
the Dock is not autohide, so they coexist. The Dock window is topmost over its
reserved rect and covers the collapsed taskbar sliver, so the taskbar never pops
up. `TaskbarService` and `DockService` are coupled only through the shared
`StateManifest`.

## Third-party traffic lights

The overlay is a **pure visual-occlusion + message-forwarding** layer. It draws our
macOS lights on the left of the caption and an opaque strip over the native
min/max/close buttons on the right. Clicks are forwarded via
`PostMessage(WM_SYSCOMMAND, SC_CLOSE/MIN/MAX)`. The target process is never
subclassed or injected.

## Overlay tracking

The overlay tracks the target with `SetWinEventHook` for
`EVENT_OBJECT_LOCATIONCHANGE`, `EVENT_SYSTEM_FOREGROUND`,
`EVENT_SYSTEM_MINIMIZESTART/END`, `EVENT_OBJECT_SHOW/HIDE`, and
`EVENT_OBJECT_DESTROY` — event-driven, never a poll loop.

## Trust

See [TRUST.md](TRUST.md). No telemetry, no network, enforced in CI.

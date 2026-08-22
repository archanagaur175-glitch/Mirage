# Revert & Uninstall

Mirage records **every** system mutation it makes into a state manifest at
`%LocalAppData%\Mirage\StateManifest.json`. Each entry carries the feature, the
API used, the operation, the previous value, and the new value. The manifest is
append-only while Mirage is active and is deleted only after a complete, successful
revert.

## Revert sequence (one click)

1. `FeatureManager.DisableAll()` turns every feature off in a single transaction.
2. `RevertSwitch.PerformRevert()` replays the manifest in **reverse order**:
   - **Taskbar** → `Taskbar.SetAutoHide(false)` restores the real taskbar.
   - **Theme** wallpaper → `SystemParametersInfo(SPI_SETDESKWALLPAPER)` with the
     previously captured path.
   - Dock / HUD / Traffic Lights leave no persistent system state once their
     windows are closed (the App layer closes them first).
3. The manifest file is deleted, leaving a clean slate.

Because every forward mutation has a corresponding reverse replay through the same
documented API, the system returns to its verified pre-Mirage state with **zero**
orphaned registry keys, hooks, services, or autostart entries.

## Uninstall

Run Revert, then delete the Mirage application folder. Nothing persists outside
`%LocalAppData%\Mirage`, which Revert removes.

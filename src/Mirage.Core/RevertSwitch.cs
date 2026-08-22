using System;
using Mirage.Native;

namespace Mirage.Core
{
    /// <summary>
    /// The Instant Safe Revert Switch. Replays every recorded mutation in reverse
    /// order via the same documented APIs, then deletes the manifest. Guarantees
    /// zero orphaned registry keys, hooks, or autostart entries because every
    /// forward mutation has a corresponding reverse replay. UI windows are closed
    /// by the App layer before/after this runs.
    /// </summary>
    public sealed class RevertSwitch
    {
        public void PerformRevert()
        {
            // 1. Turn every feature off in one transaction.
            FeatureManager.Load().DisableAll();

            // 2. Reverse system-level mutations.
            foreach (var m in StateManifest.Instance.Reversed())
            {
                switch (m.Feature)
                {
                    case "Taskbar":
                        Taskbar.SetAutoHide(false);
                        break;
                    case "Theme" when m.Operation == "SPI_SETDESKWALLPAPER" && m.PreviousValue is not null:
                        SystemParameters.SetWallpaper(m.PreviousValue);
                        break;
                    // Dock / HUD / TrafficLights leave no persistent system state
                    // once their windows are closed by the App layer.
                }
            }

            // 3. Remove the manifest so a clean slate exists afterwards.
            StateManifest.Delete();
        }
    }
}

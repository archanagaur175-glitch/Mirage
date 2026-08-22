using System;
using System.Collections.Generic;
using Mirage.Native;

namespace Mirage.Core
{
    /// <summary>
    /// Tracks the HUD feature state and records the toggle in the manifest. The
    /// actual clock / Wi-Fi / Bluetooth / battery readings happen in the App's
    /// HudWindow via WinRT device APIs (local enumeration only, no sockets).
    /// </summary>
    public sealed class HudService
    {
        public void Enable()
        {
            StateManifest.Instance.Record(new Mutation
            {
                Feature = "HUD",
                Api = "WinRTDeviceEnumeration",
                Operation = "Enable",
                NewValue = "topmost layered window",
            });
        }

        public void Disable()
        {
            StateManifest.Instance.Record(new Mutation
            {
                Feature = "HUD",
                Api = "WinRTDeviceEnumeration",
                Operation = "Disable",
            });
        }

        /// <summary>
        /// Returns the set of quick-toggle actions offered by the HUD. All are local
        /// (no network). Kept here so the matrix is testable headlessly.
        /// </summary>
        public static IReadOnlyList<string> QuickToggles()
            => new[] { "Wi-Fi", "Bluetooth", "Airplane mode", "Brightness", "Focus" };
    }
}

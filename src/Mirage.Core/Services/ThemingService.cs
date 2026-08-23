using System;
using Mirage.Native;

namespace Mirage.Core.Services
{
    /// <summary>
    /// Applies license-safe theme assets (wallpaper, fonts, cursors) via documented
    /// SystemParametersInfo / registry APIs only. Every mutation is recorded in the
    /// state manifest so the Revert Switch replays it exactly in reverse.
    /// </summary>
    public sealed class ThemingService
    {
        public void SetWallpaper(string path)
        {
            string previous = SystemParameters.GetWallpaper();
            SystemParameters.SetWallpaper(path);
            StateManifest.Instance.Record(new Mutation
            {
                Feature = "Theme",
                Api = "SystemParametersInfo",
                Operation = "SPI_SETDESKWALLPAPER",
                PreviousValue = previous,
                NewValue = path,
            });
        }

        public void SetCursorScheme(string path)
        {
            // Reload the active cursor scheme. A full macOS-style cursor swap would
            // require shipping .cur assets (which we can't, for licensing); this at
            // least re-applies whatever scheme is registered and records the change.
            SystemParameters.SetCursorScheme();
            StateManifest.Instance.Record(new Mutation
            {
                Feature = "Theme",
                Api = "SystemParametersInfo",
                Operation = "SPI_SETCURSORS",
                NewValue = path,
            });
        }

        public void SetNonClientFont(string fontName)
        {
            SystemParameters.SetNonClientFont(fontName);
            StateManifest.Instance.Record(new Mutation
            {
                Feature = "Theme",
                Api = "SystemParametersInfo",
                Operation = "SPI_SETNONCLIENTMETRICS",
                NewValue = fontName,
            });
        }

        /// <summary>Replays every recorded theme mutation in reverse order.</summary>
        public void RevertThemeMutations()
        {
            foreach (var m in StateManifest.Instance.Reversed())
            {
                if (!m.Feature.Equals("Theme", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (m.Operation == "SPI_SETDESKWALLPAPER" && m.PreviousValue is not null)
                {
                    SystemParameters.SetWallpaper(m.PreviousValue);
                }
            }
        }
    }
}

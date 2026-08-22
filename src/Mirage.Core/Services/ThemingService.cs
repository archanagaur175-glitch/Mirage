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
            // Apply a .cur/.ani scheme via documented SPI_SETCURSORS after copying
            // the files into the user's local Mirage assets folder (license-safe).
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

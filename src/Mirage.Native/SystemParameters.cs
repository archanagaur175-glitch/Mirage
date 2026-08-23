using System;
using System.Runtime.InteropServices;

namespace Mirage.Native
{
    /// <summary>
    /// Wrapper around SystemParametersInfo for license-safe theming (wallpaper,
    /// non-client metrics, cursors). Every mutation is reversible and is recorded
    /// by Mirage.Core.StateManifest so the Revert Switch can replay it exactly.
    /// </summary>
    public static class SystemParameters
    {
        public static bool SetWallpaper(string path)
        {
            IntPtr ptr = Marshal.StringToHGlobalAuto(path);
            try
            {
                return NativeMethods.SystemParametersInfo(
                    NativeConstants.SPI_SETDESKWALLPAPER,
                    0,
                    ptr,
                    NativeConstants.SPIF_UPDATEINIFILE | NativeConstants.SPIF_SENDCHANGE);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public static string GetWallpaper()
        {
            // 260 = MAX_PATH
            IntPtr ptr = Marshal.AllocHGlobal(260 * 2);
            try
            {
                bool ok = NativeMethods.SystemParametersInfo(0x0073 /* SPI_GETDESKWALLPAPER */, 260, ptr, 0);
                return ok ? Marshal.PtrToStringAuto(ptr) ?? string.Empty : string.Empty;
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public static int GetSystemMetrics(int index) => NativeMethods.GetSystemMetrics(index);

        public static bool SetNonClientFont(string fontName)
        {
            var ncm = new NONCLIENTMETRICS { cbSize = MarshalHelper.SizeOf<NONCLIENTMETRICS>() };
            if (!NativeMethods.SystemParametersInfo(0x0029 /* SPI_GETNONCLIENTMETRICS */, ncm.cbSize, ref ncm, 0))
            {
                return false;
            }

            ncm.lfCaptionFont.lfFaceName = fontName;
            ncm.lfSmCaptionFont.lfFaceName = fontName;
            ncm.lfMenuFont.lfFaceName = fontName;
            ncm.lfStatusFont.lfFaceName = fontName;
            ncm.lfMessageFont.lfFaceName = fontName;

            return NativeMethods.SystemParametersInfo(
                0x002A /* SPI_SETNONCLIENTMETRICS */,
                ncm.cbSize,
                ref ncm,
                NativeConstants.SPIF_UPDATEINIFILE | NativeConstants.SPIF_SENDCHANGE);
        }

        /// <summary>Reload the active cursor scheme (best-effort; requires the
        /// scheme/cursor files to be registered first).</summary>
        public static bool SetCursorScheme()
        {
            return NativeMethods.SystemParametersInfo(
                0x0057 /* SPI_SETCURSORS */,
                0,
                IntPtr.Zero,
                NativeConstants.SPIF_UPDATEINIFILE | NativeConstants.SPIF_SENDCHANGE);
        }
    }
}

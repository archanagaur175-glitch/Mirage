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
    }
}

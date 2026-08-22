using System;

namespace Mirage.Native
{
    /// <summary>
    /// Wrapper around Desktop Window Manager APIs used for backdrop materials
    /// (Mica/Acrylic), immersive dark mode, rounded corners and frame extension
    /// for the traffic-light title-bar decoration.
    /// </summary>
    public static class Dwm
    {
        public static void SetImmersiveDarkMode(IntPtr hWnd, bool enabled)
        {
            int value = enabled ? 1 : 0;
            NativeMethods.DwmSetWindowAttribute(hWnd, NativeConstants.DWMWA_USE_IMMERSIVE_DARK_MODE, ref value, sizeof(int));
        }

        public static void SetBackdropType(IntPtr hWnd, int backdropType)
        {
            int value = backdropType;
            NativeMethods.DwmSetWindowAttribute(hWnd, NativeConstants.DWMWA_SYSTEMBACKDROP_TYPE, ref value, sizeof(int));
        }

        public static void SetMicaEffect(IntPtr hWnd, bool enabled)
        {
            int value = enabled ? 1 : 0;
            NativeMethods.DwmSetWindowAttribute(hWnd, NativeConstants.DWMWA_MICA_EFFECT, ref value, sizeof(int));
        }

        public static void SetCornerPreference(IntPtr hWnd, int preference)
        {
            int value = preference;
            NativeMethods.DwmSetWindowAttribute(hWnd, NativeConstants.DWMWA_WINDOW_CORNER_PREFERENCE, ref value, sizeof(int));
        }

        public static int GetCloaked(IntPtr hWnd)
        {
            int value;
            NativeMethods.DwmGetWindowAttribute(hWnd, NativeConstants.DWMWA_CLOAKED, out value, sizeof(int));
            return value;
        }

        public static void ExtendFrameIntoClientArea(IntPtr hWnd, int left, int top, int right, int bottom)
        {
            var margins = new MARGINS
            {
                cxLeftWidth = left,
                cyTopHeight = top,
                cxRightWidth = right,
                cyBottomHeight = bottom,
            };
            NativeMethods.DwmExtendFrameIntoClientArea(hWnd, ref margins);
        }
    }
}

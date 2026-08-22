using System;

namespace Mirage.Native
{
    /// <summary>
    /// Wrapper around extended window-style manipulation (layered / topmost /
    /// tool-window / transparent) used for the Dock, HUD and traffic-light overlay
    /// surfaces, plus SetWindowPos helpers.
    /// </summary>
    public static class WindowStyles
    {
        public static int GetExStyle(IntPtr hWnd) => NativeMethods.GetWindowLong(hWnd, NativeConstants.GWL_EXSTYLE);

        public static void SetExStyle(IntPtr hWnd, int style)
            => NativeMethods.SetWindowLong(hWnd, NativeConstants.GWL_EXSTYLE, style);

        public static void AddExStyle(IntPtr hWnd, int flag)
        {
            int style = GetExStyle(hWnd);
            SetExStyle(hWnd, style | flag);
        }

        public static void RemoveExStyle(IntPtr hWnd, int flag)
        {
            int style = GetExStyle(hWnd);
            SetExStyle(hWnd, style & ~flag);
        }

        public static void Position(IntPtr hWnd, IntPtr insertAfter, int x, int y, int cx, int cy, uint flags)
            => NativeMethods.SetWindowPos(hWnd, insertAfter, x, y, cx, cy, flags);
    }
}

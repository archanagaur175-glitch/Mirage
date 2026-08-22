using System;

namespace Mirage.Native
{
    /// <summary>
    /// Wrapper around comctl32 window subclassing, used to override WM_NCHITTEST
    /// on Mirage-owned windows so the custom traffic-light buttons become the
    /// real close/min/max hit regions. Out-of-process windows are never subclassed.
    /// </summary>
    public static class WindowSubclass
    {
        public static bool Install(IntPtr hWnd, SUBCLASSPROC proc, uint id, IntPtr refData)
            => NativeMethods.SetWindowSubclass(hWnd, proc, id, refData);

        public static bool Remove(IntPtr hWnd, SUBCLASSPROC proc, uint id)
            => NativeMethods.RemoveWindowSubclass(hWnd, proc, id);

        public static IntPtr CallDef(IntPtr hWnd, int uMsg, IntPtr wParam, IntPtr lParam)
            => NativeMethods.DefSubclassProc(hWnd, uMsg, wParam, lParam);
    }
}

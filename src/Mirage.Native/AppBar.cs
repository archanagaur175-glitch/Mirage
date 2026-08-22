using System;

namespace Mirage.Native
{
    /// <summary>
    /// Thin, safe wrapper around the AppBar (taskbar/edge) Win32 API. Used by the
    /// Dock (to reserve the bottom edge) and by the taskbar-suppression feature
    /// (to request auto-hide on the real Explorer taskbar).
    /// </summary>
    public static class AppBar
    {
        public static IntPtr Register(IntPtr hWnd, int callbackMessage, int edge, RECT rc)
        {
            var data = new APPBARDATA
            {
                cbSize = MarshalHelper.SizeOf<APPBARDATA>(),
                hWnd = hWnd,
                uCallbackMessage = callbackMessage,
                uEdge = edge,
                rc = rc,
            };
            return NativeMethods.SHAppBarMessage(NativeConstants.ABM_NEW, ref data);
        }

        public static void SetPosition(IntPtr hWnd, int edge, RECT rc)
        {
            var data = new APPBARDATA
            {
                cbSize = MarshalHelper.SizeOf<APPBARDATA>(),
                hWnd = hWnd,
                uEdge = edge,
                rc = rc,
            };
            NativeMethods.SHAppBarMessage(NativeConstants.ABM_SETPOS, ref data);
        }

        public static void Remove(IntPtr hWnd)
        {
            var data = new APPBARDATA
            {
                cbSize = MarshalHelper.SizeOf<APPBARDATA>(),
                hWnd = hWnd,
            };
            NativeMethods.SHAppBarMessage(NativeConstants.ABM_REMOVE, ref data);
        }

        public static int GetState()
        {
            var data = new APPBARDATA
            {
                cbSize = MarshalHelper.SizeOf<APPBARDATA>(),
            };
            return (int)NativeMethods.SHAppBarMessage(NativeConstants.ABM_GETSTATE, ref data);
        }

        public static void SetState(int state)
        {
            var data = new APPBARDATA
            {
                cbSize = MarshalHelper.SizeOf<APPBARDATA>(),
                lParam = (IntPtr)state,
            };
            NativeMethods.SHAppBarMessage(NativeConstants.ABM_SETSTATE, ref data);
        }

        public static RECT GetTaskbarPosition()
        {
            var data = new APPBARDATA
            {
                cbSize = MarshalHelper.SizeOf<APPBARDATA>(),
            };
            NativeMethods.SHAppBarMessage(NativeConstants.ABM_GETTASKBARPOS, ref data);
            return data.rc;
        }
    }
}

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Mirage.Native
{
    /// <summary>
    /// Raw P/Invoke declarations. This is the single, auditable home for every
    /// native call used by Mirage. No networking or telemetry APIs live here (or
    /// anywhere in the runtime app). Delegate types are public so the wrapper
    /// classes can expose them across assemblies.
    /// </summary>
    internal static class NativeMethods
    {
        // ---- AppBar (shell32) ------------------------------------------------
        [DllImport("shell32", CharSet = CharSet.Auto)]
        public static extern IntPtr SHAppBarMessage(int dwMessage, ref APPBARDATA pData);

        // ---- SystemParametersInfo (user32) ----------------------------------
        [DllImport("user32", CharSet = CharSet.Auto)]
        public static extern bool SystemParametersInfo(int uiAction, int uiParam, IntPtr pvParam, int fWinIni);

        // ---- DWM (dwmapi) ----------------------------------------------------
        [DllImport("dwmapi")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);

        [DllImport("dwmapi")]
        public static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out int pvAttribute, int cbAttribute);

        [DllImport("dwmapi")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS pMargins);

        // ---- Window subclassing (comctl32) ----------------------------------
        [DllImport("comctl32")]
        public static extern bool SetWindowSubclass(IntPtr hWnd, SUBCLASSPROC pfnSubclass, uint uIdSubclass, IntPtr dwRefData);

        [DllImport("comctl32")]
        public static extern bool RemoveWindowSubclass(IntPtr hWnd, SUBCLASSPROC pfnSubclass, uint uIdSubclass);

        [DllImport("comctl32")]
        public static extern IntPtr DefSubclassProc(IntPtr hWnd, int uMsg, IntPtr wParam, IntPtr lParam);

        // ---- WinEvent hook (user32) -----------------------------------------
        [DllImport("user32")]
        public static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc,
            WinEventProc pfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32")]
        public static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        // ---- Misc window helpers (user32) -----------------------------------
        [DllImport("user32")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32")]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32")]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32", CharSet = CharSet.Auto)]
        public static extern uint RegisterWindowMessage(string lpString);

        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32", CharSet = CharSet.Auto)]
        public static extern int GetSystemMetrics(int nIndex);

        [DllImport("user32")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
    }

    // ---- Delegates (public so wrapper classes can expose them) --------------
    public delegate IntPtr SUBCLASSPROC(IntPtr hWnd, int uMsg, IntPtr wParam, IntPtr lParam, UIntPtr uIdSubclass, IntPtr dwRefData);
    public delegate void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
}

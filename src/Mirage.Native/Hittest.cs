using System;

namespace Mirage.Native
{
    /// <summary>
    /// Hit-test and window-enumeration helpers used by the traffic-light overlay
    /// (to find top-level foreground windows) and by the Dock (to enumerate running
    /// applications). All calls are read-only or send standard window messages;
    /// nothing is injected into other processes.
    /// </summary>
    public static class Hittest
    {
        public const int HTCLIENT = 1;
        public const int HTCAPTION = 2;
        public const int HTCLOSE = 20;
        public const int HTMINBUTTON = 21;
        public const int HTMAXBUTTON = 22;

        public static IntPtr GetForegroundWindow() => NativeMethods.GetForegroundWindow();

        public static bool GetWindowRect(IntPtr hWnd, out RECT rect)
            => NativeMethods.GetWindowRect(hWnd, out rect);

        public static int GetWindowText(IntPtr hWnd)
            => NativeMethods.GetWindowTextLength(hWnd);

        public static uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId)
            => NativeMethods.GetWindowThreadProcessId(hWnd, out processId);

        public static string GetWindowTitle(IntPtr hWnd)
        {
            int len = NativeMethods.GetWindowTextLength(hWnd);
            if (len <= 0)
            {
                return string.Empty;
            }

            var sb = new System.Text.StringBuilder(len + 1);
            NativeMethods.GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        public static void PostSysCommand(IntPtr hWnd, int command)
        {
            NativeMethods.PostMessage(hWnd, NativeConstants.WM_SYSCOMMAND, (IntPtr)command, IntPtr.Zero);
        }

        public static void ShowWindow(IntPtr hWnd, bool show)
        {
            NativeMethods.SetWindowPos(
                hWnd,
                IntPtr.Zero,
                0, 0, 0, 0,
                show ? NativeConstants.SWP_SHOWWINDOW : NativeConstants.SWP_HIDEWINDOW | NativeConstants.SWP_NOSIZE | NativeConstants.SWP_NOMOVE | NativeConstants.SWP_NOZORDER);
        }

        /// <summary>Restore (if minimized) and bring the window to the foreground.</summary>
        public static void ActivateWindow(IntPtr hWnd)
        {
            NativeMethods.ShowWindow(hWnd, 9 /* SW_RESTORE */);
            NativeMethods.SetForegroundWindow(hWnd);
        }
    }
}

using System;

namespace Mirage.Native
{
    /// <summary>
    /// Wrapper around the Explorer taskbar auto-hide request plus the
    /// TaskbarCreated watchdog registration. Mirage NEVER kills, terminates or
    /// patches explorer.exe; it only requests a documented, reversible state.
    /// </summary>
    public static class Taskbar
    {
        public static int GetAutoHideState() => AppBar.GetState();

        public static void SetAutoHide(bool autoHide)
        {
            int current = AppBar.GetState();
            if (autoHide)
            {
                AppBar.SetState(current | NativeConstants.ABS_AUTOHIDE);
            }
            else
            {
                AppBar.SetState(current & ~NativeConstants.ABS_AUTOHIDE);
            }
        }

        public static uint RegisterTaskbarCreatedMessage() =>
            NativeMethods.RegisterWindowMessage(NativeConstants.TASKBARCREATED);
    }
}

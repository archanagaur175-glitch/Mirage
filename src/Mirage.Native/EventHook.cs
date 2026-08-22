using System;

namespace Mirage.Native
{
    /// <summary>
    /// Wrapper around SetWinEventHook / UnhookWinEvent used by the third-party
    /// traffic-light overlay to track a foreign window's position, size, z-order
    /// and visibility in real time (event-driven, never polling).
    /// </summary>
    public static class EventHook
    {
        public static IntPtr Set(uint eventMin, uint eventMax, WinEventProc proc, uint idProcess = 0, uint idThread = 0,
            uint flags = NativeConstants.WINEVENT_OUTOFCONTEXT | NativeConstants.WINEVENT_SKIPOWNPROCESS)
            => NativeMethods.SetWinEventHook(eventMin, eventMax, IntPtr.Zero, proc, idProcess, idThread, flags);

        public static bool Remove(IntPtr hook) => NativeMethods.UnhookWinEvent(hook);
    }
}

using System;
using Mirage.Core.Services;
using Mirage.Native;

namespace Mirage.App;

/// <summary>
/// Drives the out-of-process traffic-light overlay for the foreground third-party
/// window. Tracks the target with SetWinEventHook (event-driven, never polling)
/// and hides the overlay when the target is minimized, hidden, cloaked or closed.
/// </summary>
public static class TrafficLightOverlayController
{
    private static TrafficLightOverlayWindow? _overlay;
    private static IntPtr _hook;
    private static WinEventProc? _callback;
    private static readonly TrafficLightService _traffic = new();

    public static void Start()
    {
        if (_overlay is not null)
        {
            return;
        }

        var target = Hittest.GetForegroundWindow();
        if (target == IntPtr.Zero || _traffic.IsExcluded(target) || BelongsToMirage(target))
        {
            return;
        }

        _overlay = new TrafficLightOverlayWindow(target);

        _callback = OnWinEvent;
        _hook = EventHook.Set(
            NativeConstants.EVENT_OBJECT_LOCATIONCHANGE,
            NativeConstants.EVENT_OBJECT_LOCATIONCHANGE,
            _callback,
            idProcess: 0,
            idThread: 0,
            flags: NativeConstants.WINEVENT_OUTOFCONTEXT);
    }

    public static void Stop()
    {
        if (_hook != IntPtr.Zero)
        {
            EventHook.Remove(_hook);
            _hook = IntPtr.Zero;
        }

        _overlay?.Close();
        _overlay = null;
        _callback = null;
    }

    private static void OnWinEvent(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        if (_overlay is null)
        {
            return;
        }

        if (hwnd != IntPtr.Zero && hwnd != _overlay.Target)
        {
            return;
        }

        switch (eventType)
        {
            case NativeConstants.EVENT_OBJECT_LOCATIONCHANGE:
                _overlay.Reposition();
                break;
            case NativeConstants.EVENT_OBJECT_HIDE:
            case NativeConstants.EVENT_OBJECT_DESTROY:
            case NativeConstants.EVENT_SYSTEM_MINIMIZESTART:
                _overlay.HideOverlay();
                break;
        }
    }

    private static bool BelongsToMirage(IntPtr hwnd)
    {
        Hittest.GetWindowThreadProcessId(hwnd, out uint pid);
        return pid == Environment.ProcessId;
    }
}

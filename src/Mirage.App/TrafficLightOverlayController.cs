using System;
using Mirage.Core.Services;
using Mirage.Native;

namespace Mirage.App;

/// <summary>
/// Drives the out-of-process traffic-light overlay for whichever third-party
/// window is currently in the foreground. Tracks foreground changes via
/// EVENT_SYSTEM_FOREGROUND (event-driven, never polling) and keeps the overlay
/// glued to that window's title-bar region. The target process is never injected
/// into or subclassed — only standard window messages are sent.
/// </summary>
public static class TrafficLightOverlayController
{
    private static TrafficLightOverlayWindow? _overlay;
    private static IntPtr _hook;
    private static WinEventProc? _callback;
    private static readonly TrafficLightService _traffic = new();

    private const uint EVENT_SYSTEM_FOREGROUND = 0x0003;

    public static void Start()
    {
        if (_callback is not null)
        {
            return;
        }

        _callback = OnWinEvent;
        _hook = EventHook.Set(
            NativeConstants.EVENT_SYSTEM_FOREGROUND,
            NativeConstants.EVENT_OBJECT_LOCATIONCHANGE,
            _callback,
            idProcess: 0,
            idThread: 0,
            flags: NativeConstants.WINEVENT_OUTOFCONTEXT);

        AttachToForeground();
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

    private static void AttachToForeground()
    {
        var fg = Hittest.GetForegroundWindow();
        if (!IsValidTarget(fg))
        {
            _overlay?.HideOverlay();
            return;
        }

        if (_overlay is null)
        {
            _overlay = new TrafficLightOverlayWindow(fg);
        }
        else
        {
            _overlay.SetTarget(fg);
        }
    }

    private static bool IsValidTarget(IntPtr hwnd)
    {
        if (hwnd == IntPtr.Zero || BelongsToMirage(hwnd) || _traffic.IsExcluded(hwnd))
        {
            return false;
        }

        return true;
    }

    private static void OnWinEvent(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        switch (eventType)
        {
            case EVENT_SYSTEM_FOREGROUND:
                AttachToForeground();
                break;

            case NativeConstants.EVENT_OBJECT_LOCATIONCHANGE:
                if (_overlay is not null && (hwnd == IntPtr.Zero || hwnd == _overlay.Target))
                {
                    _overlay.Reposition();
                }
                break;

            case NativeConstants.EVENT_OBJECT_HIDE:
            case NativeConstants.EVENT_OBJECT_DESTROY:
            case NativeConstants.EVENT_SYSTEM_MINIMIZESTART:
                if (_overlay is not null && hwnd == _overlay.Target)
                {
                    _overlay.HideOverlay();
                }
                break;
        }
    }

    private static bool BelongsToMirage(IntPtr hwnd)
    {
        Hittest.GetWindowThreadProcessId(hwnd, out uint pid);
        return pid == Environment.ProcessId;
    }
}

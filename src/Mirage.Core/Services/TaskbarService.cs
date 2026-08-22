using Mirage.Native;

namespace Mirage.Core.Services
{
    /// <summary>
    /// Requests the real Explorer taskbar to auto-hide (reversible, additive) so
    /// the Dock can visually replace it. Explores is NEVER killed, terminated or
    /// patched; only the documented ABS_AUTOHIDE state is toggled, and a watchdog
    /// re-asserts it on TaskbarCreated if the feature remains enabled.
    /// </summary>
    public sealed class TaskbarService
    {
        public void Hide()
        {
            Taskbar.SetAutoHide(true);
            StateManifest.Instance.Record(new Mutation
            {
                Feature = "Taskbar",
                Api = "SHAppBarMessage",
                Operation = "ABS_AUTOHIDE",
                PreviousValue = "normal",
                NewValue = "autohide",
            });
        }

        public void Show()
        {
            Taskbar.SetAutoHide(false);
            StateManifest.Instance.Record(new Mutation
            {
                Feature = "Taskbar",
                Api = "SHAppBarMessage",
                Operation = "ABS_NORMAL",
                PreviousValue = "autohide",
                NewValue = "normal",
            });
        }

        public uint RegisterWatchdog() => Taskbar.RegisterTaskbarCreatedMessage();

        public int CurrentState() => Taskbar.GetAutoHideState();
    }
}

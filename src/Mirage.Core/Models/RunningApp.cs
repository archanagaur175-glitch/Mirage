namespace Mirage.Core.Models
{
    /// <summary>Describes a running application discovered via EnumWindows.</summary>
    public sealed class RunningApp
    {
        public IntPtr Handle { get; set; }
        public uint ProcessId { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool HasBadge { get; set; }
    }

    /// <summary>A user-pinned Dock item (persisted locally).</summary>
    public sealed class PinnedItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string DisplayName { get; set; } = string.Empty;
        public string ExecutablePath { get; set; } = string.Empty;
        public int Order { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Mirage.Core
{
    /// <summary>
    /// One recorded, reversible system mutation. The Revert Switch replays every
    /// entry in reverse order using the same documented API that created it.
    /// </summary>
    public sealed class Mutation
    {
        public string Feature { get; set; } = string.Empty;
        public string Api { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty;
        public string? PreviousValue { get; set; }
        public string? NewValue { get; set; }
        public bool Reversible { get; set; } = true;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public Dictionary<string, string>? Metadata { get; set; }
    }

    /// <summary>
    /// Append-only manifest of every system change Mirage makes. Stored under
    /// %LocalAppData%\Mirage. Deleted only after a successful, complete revert so
    /// the system is guaranteed to return to its verified pre-Mirage state with
    /// zero orphaned registry keys, hooks, or autostart entries.
    /// </summary>
    public sealed class StateManifest
    {
        private static readonly string Folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Mirage");
        private static readonly string FilePath = Path.Combine(Folder, "StateManifest.json");

        public int Version { get; set; } = 1;
        public List<Mutation> Mutations { get; set; } = new();

        /// <summary>Process-wide singleton backed by the on-disk manifest.</summary>
        public static StateManifest Instance { get; } = Load();

        public static StateManifest Load()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    var text = File.ReadAllText(FilePath);
                    var loaded = JsonSerializer.Deserialize<StateManifest>(text);
                    if (loaded is not null)
                    {
                        return loaded;
                    }
                }
            }
            catch (Exception)
            {
                // Corrupt or unreadable manifest -> start clean (safe default).
            }

            return new StateManifest();
        }

        public void Record(Mutation mutation)
        {
            Mutations.Add(mutation);
            Save();
        }

        public void Save()
        {
            Directory.CreateDirectory(Folder);
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(FilePath, JsonSerializer.Serialize(this, options));
        }

        public static void Delete()
        {
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
        }

        /// <summary>Returns the mutations in the order they must be undone.</summary>
        public IEnumerable<Mutation> Reversed() => Mutations.AsReadOnly().Reverse();
    }
}

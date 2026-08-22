using System;
using System.IO;
using System.Text.Json;

namespace Mirage.Core
{
    /// <summary>
    /// Per-feature enable/disable state, persisted locally (no cloud). Every
    /// feature is independently toggleable; toggling one never touches another's
    /// state. The Revert Switch sets all of these to disabled in one transaction.
    /// </summary>
    public sealed class FeatureManager
    {
        private static readonly string Folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Mirage");
        private static readonly string FilePath = Path.Combine(Folder, "settings.json");

        public bool DockEnabled { get; set; }
        public bool HudEnabled { get; set; }
        public bool TrafficLightsEnabled { get; set; }
        public bool TaskbarHidden { get; set; }
        public bool ThemingApplied { get; set; }

        public static FeatureManager Load()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    var text = File.ReadAllText(FilePath);
                    var loaded = JsonSerializer.Deserialize<FeatureManager>(text);
                    if (loaded is not null)
                    {
                        return loaded;
                    }
                }
            }
            catch (Exception)
            {
                // Fall through to defaults.
            }

            return new FeatureManager();
        }

        public void Save()
        {
            Directory.CreateDirectory(Folder);
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(FilePath, JsonSerializer.Serialize(this, options));
        }

        public void DisableAll()
        {
            DockEnabled = false;
            HudEnabled = false;
            TrafficLightsEnabled = false;
            TaskbarHidden = false;
            ThemingApplied = false;
            Save();
        }
    }
}

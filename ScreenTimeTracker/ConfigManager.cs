using System;
using System.IO;
using System.Text.Json;

namespace ScreenTimeTracker
{
    public class TrackerConfig
    {
        public int IdleThresholdMinutes { get; set; } = 5;
        public int PollingIntervalSeconds { get; set; } = 5;
        public bool EnableEncryption { get; set; } = true;
        public string LogDirectory { get; set; } = "Logs";
    }

    public static class ConfigManager
    {
        private static readonly string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
        public static TrackerConfig Settings { get; private set; } = new TrackerConfig();

        public static void LoadConfig()
        {
            if (!File.Exists(configPath))
            {
                SaveDefaultConfig();
                Console.WriteLine("Default config created.");
            }

            try
            {
                string json = File.ReadAllText(configPath);
                Settings = JsonSerializer.Deserialize<TrackerConfig>(json) ?? new TrackerConfig();
                Console.WriteLine("Config loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load config: {ex.Message}");
                SaveDefaultConfig();
            }
        }

        private static void SaveDefaultConfig()
        {
            string json = JsonSerializer.Serialize(Settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(configPath, json);
        }
    }
}

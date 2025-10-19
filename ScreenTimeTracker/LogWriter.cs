using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace ScreenTimeTracker
{
    public class ActivityLog
    {
        public DateTime Timestamp { get; set; }
        public string? Status { get; set; } // "Active" or "Idle"
        public string? AppName { get; set; }
        public string? WindowTitle { get; set; }
        public TimeSpan IdleDuration { get; set; }
    }

    public static class LogWriter
    {
        private static readonly string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        private static readonly string logFile = Path.Combine(logDirectory, $"{DateTime.Now:yyyy-MM-dd}.json");

        public static void WriteLog(ActivityLog entry)
        {
            try
            {
                if (!Directory.Exists(logDirectory))
                    Directory.CreateDirectory(logDirectory);

                List<ActivityLog> logs = new List<ActivityLog>();

                if (File.Exists(logFile))
                {
                    //string existing = File.ReadAllText(logFile);
                    //logs = JsonSerializer.Deserialize<List<ActivityLog>>(existing) ?? new List<ActivityLog>();
                    byte[] encrypted = File.ReadAllBytes(logFile);
                    string decrypted = Encryptor.Decrypt(encrypted);
                    logs = JsonSerializer.Deserialize<List<ActivityLog>>(decrypted) ?? new List<ActivityLog>();

                }

                logs.Add(entry);
                string json = JsonSerializer.Serialize(logs, new JsonSerializerOptions { WriteIndented = true });
                //File.WriteAllText(logFile, json);
                byte[] encrypted1 = Encryptor.Encrypt(json);
                File.WriteAllBytes(logFile, encrypted1);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Log write failed: {ex.Message}");
            }
        }
    }
}

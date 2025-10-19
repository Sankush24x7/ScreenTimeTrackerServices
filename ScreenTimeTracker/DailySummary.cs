using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

namespace ScreenTimeTracker
{
    public static class DailySummary
    {
        public static DailySummaryResult GenerateSummary(DateTime date)
        {
            string logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", $"{date:yyyy-MM-dd}.json");

            if (!File.Exists(logFile))
                return new DailySummaryResult { Date = date };

            try
            {
                byte[] encrypted = File.ReadAllBytes(logFile);
                string decrypted = Encryptor.Decrypt(encrypted);
                List<ActivityLog> logs = JsonSerializer.Deserialize<List<ActivityLog>>(decrypted) ?? new();

                TimeSpan totalActive = TimeSpan.Zero;
                TimeSpan totalIdle = TimeSpan.Zero;
                Dictionary<string, TimeSpan> appUsage = new();

                for (int i = 1; i < logs.Count; i++)
                {
                    var prev = logs[i - 1];
                    var curr = logs[i];
                    TimeSpan duration = curr.Timestamp - prev.Timestamp;

                    if (prev.Status == "Active")
                    {
                        totalActive += duration;
                        string key = $"{prev.AppName} - {prev.WindowTitle}";
                        if (!appUsage.ContainsKey(key))
                            appUsage[key] = TimeSpan.Zero;
                        appUsage[key] += duration;
                    }
                    else
                    {
                        totalIdle += duration;
                    }
                }

                return new DailySummaryResult
                {
                    Date = date,
                    TotalActive = totalActive,
                    TotalIdle = totalIdle,
                    AppUsage = appUsage
                };
            }
            catch
            {
                return new DailySummaryResult { Date = date };
            }
        }
    }
}

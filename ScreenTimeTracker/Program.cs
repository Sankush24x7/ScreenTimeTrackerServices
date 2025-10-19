using ScreenTimeTracker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("🟢 Screen Time Tracker started...");
        TimeSpan idleThreshold = TimeSpan.FromMinutes(5);
        string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        Directory.CreateDirectory(logDirectory);

        DateTime currentDate = DateTime.Today;
        string logFile = Path.Combine(logDirectory, $"{currentDate:yyyy-MM-dd}.json");

        // Create empty encrypted log file if missing
        if (!File.Exists(logFile))
        {
            Console.WriteLine($"📁 Creating new log file for {currentDate:yyyy-MM-dd}");
            File.WriteAllBytes(logFile, Encryptor.Encrypt("[]"));
        }

        while (true)
        {
            DateTime now = DateTime.Now;

            // 🔄 Daily rollover
            if (now.Date > currentDate)
            {
                string previousLogFile = Path.Combine(logDirectory, $"{currentDate:yyyy-MM-dd}.json");
                if (File.Exists(previousLogFile))
                {
                    byte[] prevEncrypted = File.ReadAllBytes(previousLogFile);
                    string prevDecrypted = Encryptor.Decrypt(prevEncrypted);
                    List<ActivityLog> prevLogs = JsonSerializer.Deserialize<List<ActivityLog>>(prevDecrypted) ?? new();
                    DailySummaryResult prevSummary = DailySummary.GenerateSummary(currentDate);
                    HtmlBuilder.GenerateReport(prevSummary, prevLogs);
                }

                currentDate = now.Date;
                logFile = Path.Combine(logDirectory, $"{currentDate:yyyy-MM-dd}.json");
                File.WriteAllBytes(logFile, Encryptor.Encrypt("[]"));
                Console.WriteLine($"📅 New day detected. Started log for {currentDate:yyyy-MM-dd}");
            }

            // 🕵️ Capture snapshot
            string activeApp = AppLogger.GetActiveProcessName();
            string windowTitle = AppLogger.GetActiveWindowTitle();
            TimeSpan idleTime = IdleDetector.GetIdleTime();
            bool isIdle = idleTime > idleThreshold;

            ActivityLog logEntry = new ActivityLog
            {
                Timestamp = now,
                Status = isIdle ? "Idle" : "Active",
                AppName = activeApp,
                WindowTitle = windowTitle,
                IdleDuration = idleTime
            };

            // 🔐 Read + append log
            byte[] encrypted = File.ReadAllBytes(logFile);
            string decrypted = Encryptor.Decrypt(encrypted);

            if (string.IsNullOrWhiteSpace(decrypted))
            {
                Console.WriteLine("⚠️ Decrypted log is empty or invalid.");
                decrypted = "[]"; // fallback to empty JSON array
            }

            List<ActivityLog> logs;
            try
            {
                logs = JsonSerializer.Deserialize<List<ActivityLog>>(decrypted) ?? new();
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"❌ JSON deserialization failed: {ex.Message}");
                logs = new(); // fallback to empty list
            }

            // ✅ Append new entry
            logs.Add(logEntry);

            // 🔐 Save updated log
            string updatedJson = JsonSerializer.Serialize(logs);
            byte[] updatedEncrypted = Encryptor.Encrypt(updatedJson);
            File.WriteAllBytes(logFile, updatedEncrypted);

            // 📊 Generate live report
            DailySummaryResult summary = DailySummary.GenerateSummary(currentDate);
            //HtmlBuilder.GenerateReport(summary, logs);
            try
            {
                HtmlBuilder.GenerateReport(summary, logs);
            }
            catch (IOException ex)
            {
                File.AppendAllText("error_log.txt", $"{DateTime.Now}: Could not write HTML report → {ex.Message}\n");
            }


            Console.WriteLine($"{now:HH:mm:ss} | {(isIdle ? "Idle" : "Active")} | App: {activeApp} | Title: {windowTitle}");

            Thread.Sleep(5000); // Poll every 5 seconds
        }
    }
}

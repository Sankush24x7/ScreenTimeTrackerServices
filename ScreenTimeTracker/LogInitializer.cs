using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ScreenTimeTracker
{
    public static class LogInitializer
    {
        public static void EnsureLogExists(DateTime date)
        {
            string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);

            string logFile = Path.Combine(logDirectory, $"{date:yyyy-MM-dd}.json");
            if (File.Exists(logFile)) return;

            var dummyLog = new List<ActivityLog> { /* ... */ };
            string json = JsonSerializer.Serialize(dummyLog);
            byte[] encrypted = Encryptor.Encrypt(json);
            File.WriteAllBytes(logFile, encrypted);
        }
    }

}

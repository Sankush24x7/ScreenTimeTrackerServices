using System;

namespace ScreenTimeTracker
{
    public static class Tracker
    {
        public static void Run()
        {
            var config = ConfigManager.Settings;
            TimeSpan idleThreshold = TimeSpan.FromMinutes(config.IdleThresholdMinutes);

            string appName = AppLogger.GetActiveProcessName();
            string windowTitle = AppLogger.GetActiveWindowTitle();
            TimeSpan idleTime = IdleDetector.GetIdleTime();
            bool isIdle = idleTime > idleThreshold;

            ActivityLog logEntry = new ActivityLog
            {
                Timestamp = DateTime.Now,
                Status = isIdle ? "Idle" : "Active",
                AppName = appName,
                WindowTitle = windowTitle,
                IdleDuration = idleTime
            };

            LogWriter.WriteLog(logEntry);
        }
    }
}

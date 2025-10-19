using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ScreenTimeTracker.Models;

namespace ScreenTimeTracker.Reporting
{
    public static class SummaryLoader
    {
        public static List<EDailySummaryResult> LoadAllSummaries(string reportDir)
{
    var summaries = new List<EDailySummaryResult>();
    var files = Directory.GetFiles(reportDir, "summary_*.json");

    Console.WriteLine($"📁 Found {files.Length} summary files in {reportDir}");

    foreach (var file in files)
    {
        Console.WriteLine($"📄 Reading: {Path.GetFileName(file)}");

        string json = File.ReadAllText(file);
        var summary = JsonSerializer.Deserialize<EDailySummaryResult>(json);

        if (summary != null && summary.AppSummaries.Count > 0)
        {
            Console.WriteLine($"✅ Loaded summary for {summary.Date:yyyy-MM-dd} with {summary.AppSummaries.Count} apps");
            summaries.Add(summary);
        }
        else
        {
            Console.WriteLine($"⚠️ Empty or invalid summary in {file}");
        }
    }

    return summaries;
}

    }
}

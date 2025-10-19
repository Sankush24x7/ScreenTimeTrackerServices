using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace ScreenTimeTracker
{
    public static class HtmlBuilder
    {
        public static void GenerateReport(DailySummaryResult summary, List<ActivityLog> logs)
        {
            string labels = string.Join(",", summary.AppUsage.Keys.Select(k => $"\"{k}\""));
            string data = string.Join(",", summary.AppUsage.Values.Select(v => Math.Round(v.TotalMinutes)));

            var sb = new StringBuilder();
            sb.AppendLine("<html><head><title>Screen Time Report</title>");
            sb.AppendLine("<script src=\"https://cdn.jsdelivr.net/npm/chart.js\"></script>");
            sb.AppendLine("<style>");
            sb.AppendLine("body { font-family: Arial; margin: 20px; }");
            sb.AppendLine("table { border-collapse: collapse; width: 100%; margin-top: 10px; }");
            sb.AppendLine("th, td { border: 1px solid #ccc; padding: 8px; text-align: left; }");
            sb.AppendLine("th { background-color: #f2f2f2; }");
            sb.AppendLine("</style></head><body>");

            sb.AppendLine($"<h2>📅 Summary for {summary.Date:yyyy-MM-dd}</h2>");
            sb.AppendLine($"<p>🟢 Active Time: {summary.TotalActive}</p>");
            sb.AppendLine($"<p>🔴 Idle Time: {summary.TotalIdle}</p>");

            // Chart
            sb.AppendLine("<canvas id=\"usageChart\" width=\"800\" height=\"400\"></canvas>");
            sb.AppendLine("<script>");
            sb.AppendLine("const ctx = document.getElementById('usageChart').getContext('2d');");
            sb.AppendLine("const chart = new Chart(ctx, { type: 'bar', data: {");
            sb.AppendLine($"labels: [{labels}],");
            sb.AppendLine("datasets: [{ label: 'App Usage (minutes)', data: [" + data + "], backgroundColor: 'rgba(54, 162, 235, 0.6)' }]");
            sb.AppendLine("}});</script>");

            // App usage table
            sb.AppendLine("<h3>📂 App Usage Breakdown</h3>");
            sb.AppendLine("<table><tr><th>App</th><th>Duration</th></tr>");
            foreach (var app in summary.AppUsage.OrderByDescending(a => a.Value))
            {
                sb.AppendLine($"<tr><td>{app.Key}</td><td>{app.Value}</td></tr>");
            }
            sb.AppendLine("</table>");

            // Filters
            sb.AppendLine("<h3>🔍 Filter Logs</h3>");
            sb.AppendLine("<label for='appFilter'>App Name:</label>");
            sb.AppendLine("<input type='text' id='appFilter' onkeyup='filterLogs()' placeholder='Type to filter by app...'>");

            sb.AppendLine("<label for='timeFilter'>Start Time:</label>");
            sb.AppendLine("<input type='time' id='timeFilter' onchange='filterLogs()'>");

            // Log table
            sb.AppendLine("<h3>📋 Raw Activity Log</h3>");
            sb.AppendLine("<table id='logTable'><tr><th>Time</th><th>Status</th><th>App</th><th>Window Title</th><th>Idle Duration</th></tr>");
            foreach (var log in logs)
            {
                sb.AppendLine($"<tr class='logRow'><td>{log.Timestamp:HH:mm:ss}</td><td>{log.Status}</td><td>{log.AppName}</td><td>{log.WindowTitle}</td><td>{log.IdleDuration}</td></tr>");
            }
            sb.AppendLine("</table>");

            // Filter script
            sb.AppendLine("<script>");
            sb.AppendLine("function filterLogs() {");
            sb.AppendLine("  const appInput = document.getElementById('appFilter').value.toLowerCase();");
            sb.AppendLine("  const timeInput = document.getElementById('timeFilter').value;");
            sb.AppendLine("  const rows = document.querySelectorAll('.logRow');");
            sb.AppendLine("  rows.forEach(row => {");
            sb.AppendLine("    const app = row.cells[2].textContent.toLowerCase();");
            sb.AppendLine("    const time = row.cells[0].textContent;");
            sb.AppendLine("    const matchesApp = app.includes(appInput);");
            sb.AppendLine("    const matchesTime = !timeInput || time >= timeInput;");
            sb.AppendLine("    row.style.display = (matchesApp && matchesTime) ? '' : 'none';");
            sb.AppendLine("  });");
            sb.AppendLine("}");
            sb.AppendLine("</script>");

            sb.AppendLine("</body></html>");

            //string outputPath = Path.Combine("Reports", $"report_{summary.Date:yyyyMMdd}.html");
            //Directory.CreateDirectory("Reports");
            //File.WriteAllText(outputPath, sb.ToString());
            string reportFileName = $"report_{summary.Date:yyyyMMdd}.html";
            string outputDir = Path.Combine(AppContext.BaseDirectory, "Reports");
            Directory.CreateDirectory(outputDir);

            string outputPath = Path.Combine(outputDir, reportFileName);
            string htmlContent = sb.ToString(); // assuming sb is your StringBuilder

            int retries = 3;
            for (int i = 0; i < retries; i++)
            {
                try
                {
                    using var stream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.Read);
                    using var writer = new StreamWriter(stream);
                    writer.Write(htmlContent);
                    break; // success
                }
                catch (IOException ex)
                {
                    if (i == retries - 1)
                    {
                        // Final failure — log and optionally rethrow
                        File.AppendAllText("error_log.txt", $"{DateTime.Now}: Failed to write HTML report → {ex.Message}\n");
                        throw;
                    }

                    // Wait before retrying
                    Thread.Sleep(500);
                }
            }


            Console.WriteLine($"✅ Report saved to {outputPath}");
        }
    }
}

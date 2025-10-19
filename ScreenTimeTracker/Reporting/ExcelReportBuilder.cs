using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using ClosedXML.Excel;
using ScreenTimeTracker.Models;


namespace ScreenTimeTracker.Reporting
{
    public static class ExcelReportBuilder
    {
        public static string BuildConsolidatedReport(List<EDailySummaryResult> allDays, string outputPath)
        {
            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("ScreenTime Summary");

            sheet.Cell(1, 1).Value = "Date";
            sheet.Cell(1, 2).Value = "App Name";
            sheet.Cell(1, 3).Value = "Duration (minutes)";
            sheet.Cell(1, 4).Value = "Idle Time";
            sheet.Cell(1, 5).Value = "Active Time";

            int row = 2;
            foreach (var day in allDays)
            {
                foreach (var app in day.AppSummaries)
                {
                    sheet.Cell(row, 1).Value = day.Date.ToString("yyyy-MM-dd");
                    sheet.Cell(row, 2).Value = app.AppName;
                    sheet.Cell(row, 3).Value = app.TotalMinutes;
                    sheet.Cell(row, 4).Value = app.IdleMinutes;
                    sheet.Cell(row, 5).Value = app.ActiveMinutes;
                    row++;
                }
            }

            string filePath = Path.Combine(outputPath, $"ConsolidatedReport_{DateTime.Now:yyyyMMdd}.xlsx");
            workbook.SaveAs(filePath);
            return filePath;
        }
    }

}

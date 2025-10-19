using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ScreenTimeTracker
{
    public static class EmailSummaryBuilder
    {
        public static string BuildHtml(DailySummaryResult summary)
        {
            var sb = new StringBuilder();
            sb.Append($"<h2>📅 Summary for {summary.Date:yyyy-MM-dd}</h2>");
            sb.Append($"<p>🟢 Active Time: {summary.TotalActive}</p>");
            sb.Append($"<p>🔴 Idle Time: {summary.TotalIdle}</p>");
            sb.Append("<ul>");
            foreach (var app in summary.AppUsage.OrderByDescending(a => a.Value))
            {
                sb.Append($"<li>{app.Key}: {app.Value}</li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }
        public static void SendSummary(string htmlBody)
        {
            var msg = new MailMessage("from@example.com", "to@example.com", "Daily Screen Time Summary", "")
            {
                IsBodyHtml = true,
                Body = htmlBody
            };

            var client = new SmtpClient("smtp.example.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("username", "password"),
                EnableSsl = true
            };

            client.Send(msg);
        }
    }

}

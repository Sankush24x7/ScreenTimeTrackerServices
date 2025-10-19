using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScreenTimeTracker.Models;

namespace ScreenTimeTracker.Models
{
    public class EDailySummaryResult
    {
        public DateTime Date { get; set; }

        public List<AppSummary> AppSummaries { get; set; } = new();
    }

}

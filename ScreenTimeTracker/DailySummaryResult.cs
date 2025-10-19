using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenTimeTracker
{
    public class DailySummaryResult
    {
        public DateTime Date { get; set; }
        public TimeSpan TotalActive { get; set; }
        public TimeSpan TotalIdle { get; set; }
        public Dictionary<string, TimeSpan> AppUsage { get; set; } = new();
    }
    
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenTimeTracker.Models
{
    public class AppSummary
    {
        public string AppName { get; set; } = string.Empty;
        public int TotalMinutes { get; set; }
        public int IdleMinutes { get; set; }
        public int ActiveMinutes => TotalMinutes - IdleMinutes;
    }

}

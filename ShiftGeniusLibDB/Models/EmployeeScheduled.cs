using System;
using System.Collections.Generic;

namespace ShiftGeniusLibDB.Models
{
    public partial class EmployeeScheduled
    {
        public int EmployeeScheduledId { get; set; }
        public int ScheduleDayId { get; set; }
        public int? EmployeeRoleId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public virtual EmployeeRole EmployeeRole { get; set; } = null!;
        public virtual ScheduleDay ScheduleDay { get; set; } = null!;
    }
}

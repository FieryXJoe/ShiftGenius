using System;
using System.Collections.Generic;

namespace ShiftGeniusLibDB.Models
{
    public partial class ScheduleDay
    {
        public ScheduleDay()
        {
            EmployeeScheduleds = new HashSet<EmployeeScheduled>();
        }

        public int ScheduleDayId { get; set; }
        public int OrganizationId { get; set; }
        public DateTime Day { get; set; }

        public virtual Organization Organization { get; set; } = null!;
        public virtual ICollection<EmployeeScheduled> EmployeeScheduleds { get; set; }
    }
}

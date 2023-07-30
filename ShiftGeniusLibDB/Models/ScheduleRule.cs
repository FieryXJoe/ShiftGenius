using System;
using System.Collections.Generic;

namespace ShiftGeniusLibDB.Models
{
    public partial class ScheduleRule
    {
        public int ScheduleRuleId { get; set; }
        public int OrganizationId { get; set; }
        public int? EmployeeId { get; set; }
        public string? Rule { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime DateCreated { get; set; }
        public int? CreatedBy { get; set; }
        public bool Approved { get; set; }

        public virtual EmployeeRole? CreatedByNavigation { get; set; }
        public virtual Employee? Employee { get; set; }
        public virtual Organization Organization { get; set; } = null!;
    }
}

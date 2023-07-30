using System;
using System.Collections.Generic;

namespace ShiftGeniusLibDB.Models
{
    public partial class EmployeeRole
    {
        public EmployeeRole()
        {
            EmployeeScheduleds = new HashSet<EmployeeScheduled>();
            ScheduleRules = new HashSet<ScheduleRule>();
        }

        public int EmployeeRoleId { get; set; }
        public int EmployeeId { get; set; }
        public int RoleId { get; set; }

        public virtual Employee Employee { get; set; } = null!;
        public virtual Role Role { get; set; } = null!;
        public virtual ICollection<EmployeeScheduled> EmployeeScheduleds { get; set; }
        public virtual ICollection<ScheduleRule> ScheduleRules { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace ShiftGeniusLibDB.Models
{
    public partial class Organization
    {
        public Organization()
        {
            Employees = new HashSet<Employee>();
            ScheduleDays = new HashSet<ScheduleDay>();
            ScheduleRules = new HashSet<ScheduleRule>();
        }

        public int OrganizationId { get; set; }
        public int Owner { get; set; }
        public DateTime? SubscriptionEnd { get; set; }

        public virtual Employee OwnerNavigation { get; set; } = null!;
        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<ScheduleDay> ScheduleDays { get; set; }
        public virtual ICollection<ScheduleRule> ScheduleRules { get; set; }
    }
}

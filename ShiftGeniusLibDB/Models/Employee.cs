using System;
using System.Collections.Generic;

namespace ShiftGeniusLibDB.Models
{
    public partial class Employee
    {
        public Employee()
        {
            EmployeeRoles = new HashSet<EmployeeRole>();
            Organizations = new HashSet<Organization>();
            ScheduleRules = new HashSet<ScheduleRule>();
        }

        public int EmployeeId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int? OrganizationId { get; set; }
        public string? Password { get; set; }

        public virtual Organization? Organization { get; set; }
        public virtual ICollection<EmployeeRole> EmployeeRoles { get; set; }
        public virtual ICollection<Organization> Organizations { get; set; }
        public virtual ICollection<ScheduleRule> ScheduleRules { get; set; }
    }
}

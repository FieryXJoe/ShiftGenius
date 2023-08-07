using System;
using System.Collections.Generic;

namespace ShiftGeniusLibDB.Models
{
    public partial class Role
    {
        public Role()
        {
            EmployeeRoles = new HashSet<EmployeeRole>();
        }

        public int RoleId { get; set; }
        public int OrganizationId { get; set; }
        public string? Name { get; set; }
        public bool IsManager { get; set; }

        public virtual ICollection<EmployeeRole> EmployeeRoles { get; set; }
    }
}

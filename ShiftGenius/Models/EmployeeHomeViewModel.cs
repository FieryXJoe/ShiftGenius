using Microsoft.Azure.Management.Redis.Fluent.Models;

namespace ShiftGenius.Models
{
    public class EmployeeHomeViewModel
    {
        public string EmployeeName { get; set; }
        public List<ScheduleEntry> Schedule { get; set; } // we need to define this class
    }
}


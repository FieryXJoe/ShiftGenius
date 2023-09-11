using System;

namespace ShiftGeniusLibDB.Models
{
    public class TimeOffRequest
    {
        public int RequestID { get; set; } // Primary key, typically auto-incrementing
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Type { get; set; }
        public DateTime RequestDate { get; set; }
        public int EmployeeID { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShiftGeniusLibDB.Models
{
    public class TimeOffRequest
    {
        [Key]
        public int RequestID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Type { get; set; }
        public DateTime RequestDate { get; set; }
        public int EmployeeID { get; set; }
        public string? Status { get; set; }
    }
}

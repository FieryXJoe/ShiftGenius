﻿namespace ShiftGenius.Models
{
    public class EmployeeTimeOffRequestViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Type { get; set; } // nullable

    }

}

using ShiftGenius.ShiftGenius.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftGenius.Models
{
    public class ManagerTimeOffRequestsViewModel
    {
        public List<TimeOffRequestViewModel>? TimeOffRequests { get; set; }
    }

    public class TimeOffRequestViewModel
    {
        [Key]
        public int RequestID { get; set; }
        public string EmployeeID { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Type { get; set; }
        public DateTime RequestDate { get; set; }
        public string? Status { get; set; }
    }
}

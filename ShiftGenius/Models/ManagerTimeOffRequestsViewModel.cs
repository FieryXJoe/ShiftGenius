using System.Collections.Generic;
namespace ShiftGenius.Models
{
    public class ManagerTimeOffRequestsViewModel
    {
        public List<TimeOffRequest> TimeOffRequests { get; set; }
    }

    public class TimeOffRequest
    {
        public string EmployeeName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string TypeOfTimeOff { get; set; }

    }
}

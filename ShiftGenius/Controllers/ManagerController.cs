using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftGenius.Models;
using System.Collections.Generic;

namespace ShiftGenius.Controllers
{
    public class ManagerController : Controller
    {
        [Authorize(Policy = "IsManager")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Policy = "IsManager")]
        public IActionResult ManagerTimeOffRequests()
        {
            var viewModel = new ManagerTimeOffRequestsViewModel
            {
                TimeOffRequests = GetMockTimeOffRequests() // Replace with the logic to retrieve time-off requests
            };

            return View(viewModel);
        }

        [Authorize(Policy = "IsManager")]
        public IActionResult ManagerAvailabilityRequests()
        {
            var availabilityRequests = GetAvailabilityRequests(); // Replace with the logic to retrieve availability requests
            return View(availabilityRequests);
        }

        // This is just a mock function to generate sample data, replace it with actual data retrieval logic
        
        private List<TimeOffRequest> GetMockTimeOffRequests()
        {
            return new List<TimeOffRequest>
            {
                new TimeOffRequest { EmployeeName = "Joe", StartDate = "2023-08-15", EndDate = "2023-08-17", TypeOfTimeOff = "Vacation" },
                new TimeOffRequest { EmployeeName = "Mike", StartDate = "2023-08-18", EndDate = "2023-08-20", TypeOfTimeOff = "Sick Leave" }
            };
        }

        //Logic to be updating the employee's avail
        private List<ManagerAvailRequestModel> GetAvailabilityRequests()
        {
            return new List<ManagerAvailRequestModel>
            {
                new ManagerAvailRequestModel { EmployeeID = "001", EmployeeName = "Mike Mike" },
                new ManagerAvailRequestModel { EmployeeID = "002", EmployeeName = "Joe Joe" },
            };
        }
    }
}

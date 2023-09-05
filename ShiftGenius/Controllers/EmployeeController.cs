using Microsoft.AspNetCore.Mvc;
using ShiftGenius.Models;
using ShiftGeniusLibDB.Models;

namespace ShiftGenius.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            //Employee employee = GetCurrentEmployee(); // Replace with logic

            // Create a ViewModel to hold the data
            //var viewModel = new EmployeeHomeViewModel
            //{
            //    EmployeeName = employee.Name,
            //    Schedule = GetEmployeeSchedule(employee.Id), // Replace with logic
            //};

            //return View(viewModel);
            return View();
        }

        public IActionResult EmployeeTimeOffRequest()
        {
            return View();
        }

        public IActionResult EmployeeAvailChange()
        {
            // For demonstration purposes, let's create a sample availability dictionary
            var availabilityData = new Dictionary<string, Availability>
            {
                { "Monday", new Availability { Enabled = true, StartTime = "09:00", EndTime = "17:00" } },
                { "Tuesday", new Availability { Enabled = false, StartTime = "10:00", EndTime = "18:00" } },
                { "Wednesday", new Availability { Enabled = true, StartTime = "09:00", EndTime = "17:00" } },
                { "Thursday", new Availability { Enabled = false, StartTime = "10:00", EndTime = "18:00" } },
                { "Friday", new Availability { Enabled = false, StartTime = "6:00", EndTime = "14:00" } },
                // Add more days as needed
            };

            var viewModel = new EmployeeAvailChangeModel
            {
                Availability = availabilityData
            };

            return View(viewModel);
        }
    }
}

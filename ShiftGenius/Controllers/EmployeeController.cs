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

    }
}

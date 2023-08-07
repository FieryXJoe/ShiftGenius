using Microsoft.AspNetCore.Mvc;
using ShiftGenius.Models;

namespace ShiftGenius.Controllers
{
    public class EmployeeTimeOffRequestController : Controller
    {
        public IActionResult EmployeeTimeOffRequest()
        {
            // This action displays the time-off request form
            return View();
        }
        public IActionResult EmployeeTimeOffRequest(EmployeeTimeOffRequestViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Process the time-off request, save to database, etc.
                // Redirect to a confirmation page or back to the home page
                return RedirectToAction("Index");
            }

            return View(model); // Return to the view with validation errors
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using ShiftGenius.Models;
using System.Diagnostics;

using ShiftGeniusLibDB;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
public class EmployeeController : Controller
{

    [HttpGet]
    public IActionResult EmployeeTimeOffRequest()
    {
        // This action displays the time-off request form
        return View();
    }

    [HttpPost]
    public IActionResult EmployeeTimeOffRequest(EmployeeTimeOffRequestViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Process the time-off request, save to database, etc.
            // Redirect to a confirmation page or back to the home page
            return RedirectToAction("EmployeeHome");
        }

        return View(model); // Return to the view with validation errors
    }
}

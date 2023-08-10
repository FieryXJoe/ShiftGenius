using Microsoft.AspNetCore.Mvc;
using ShiftGenius.Models;

namespace ShiftGenius.Controllers
{
	public class EmployeeAvailChangeController : Controller
	{
		public IActionResult EmployeeAvailChange()
		{
			var model = new EmployeeAvailChangeModel();
			// Initialize the model if needed
			return View(model);
		}

		[HttpPost]
		public IActionResult SubmitAvailChange(EmployeeAvailChangeModel model)
		{
			if (ModelState.IsValid)
			{
				// Process the availability change, save to database, etc.
				// Redirect to a confirmation page or back to the home page
				return RedirectToAction("Index", "Home");
			}

			return View("EmployeeAvailChange", model); // Return to the view with validation errors
		}
	}
}

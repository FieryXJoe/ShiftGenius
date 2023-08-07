using Microsoft.AspNetCore.Mvc;
using ShiftGenius.Models;
using System.Diagnostics;

using ShiftGeniusLibDB;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

public IActionResult EmployeeHome()
{
	// Load the employee's name and schedule data (you need to implement this)
	Employee employee = GetCurrentEmployee(); // Replace with your logic

	// Create a ViewModel to hold the data
	var viewModel = new EmployeeHomeViewModel
	{
		EmployeeName = employee.Name,
		Schedule = GetEmployeeSchedule(employee.Id), // Replace with your logic
	};

	return View(viewModel);
}

﻿using Microsoft.AspNetCore.Mvc;
using ShiftGenius.Models;

namespace ShiftGenius.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _dbContext;

        public EmployeeController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            // Replace with your logic to fetch employee data
            // var employee = GetCurrentEmployee();

            // Create a ViewModel to hold the data
            // var viewModel = new EmployeeHomeViewModel
            // {
            //     EmployeeName = employee.Name,
            //     Schedule = GetEmployeeSchedule(employee.Id), // Replace with logic
            // };

            // Return the view model
            // return View(viewModel);

            return View();
        }

        public IActionResult EmployeeTimeOffRequest()
        {
            return View();
        }

        [HttpPost]
        public IActionResult EmployeeTimeOffRequest(EmployeeTimeOffRequestModel model)
        {
            if (ModelState.IsValid)
            {
                var timeOffRequest = new ShiftGeniusLibDB.Models.TimeOffRequest
                {
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    Type = model.Type,
                    RequestDate = DateTime.Now,
                    Status = "Pending", // Set the initial status
                    EmployeeID = 1, // Replace with the actual employee ID
                };

                _dbContext.TimeOffRequests.Add(timeOffRequest);
                _dbContext.SaveChanges();

                // Set a success flag in TempData
                TempData["IsSuccess"] = true;

                return RedirectToAction("Index", "Employee");
            }

            return View(model);
        }


        public IActionResult EmployeeAvailChange()
        {
            // For demonstration purposes, let's create a sample availability dictionary
            var availabilityData = new Dictionary<string, Availability>
            {
                // Add availability data here
            };
            var viewModel = new EmployeeAvailChangeModel
            {
                Availability = availabilityData
            };

            return View(viewModel);
        }
    }
}

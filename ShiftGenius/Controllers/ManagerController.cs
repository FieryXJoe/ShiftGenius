using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShiftGenius.Models;
using ShiftGenius.Rules;
using ShiftGeniusLibDB;
using ShiftGeniusLibDB.Aggregate;
using ShiftGeniusLibDB.Models;
using System.Collections.Generic;
using System.Data;
using System.Security.Claims;

namespace ShiftGenius.Controllers
{
    public class ManagerController : Controller
    {
        [Authorize(Policy = "IsManager")]
        public IActionResult Index()
        {
            return View();
        }
        public ActionResult ScheduleGenerator()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);
            int organizationID = Basic_Functions.getEmployeeByID(userId).OrganizationId.Value;

            Schedule schedule = new Schedule(organizationID);
            WeeklyScheduleViewModel model = new WeeklyScheduleViewModel
            {
                Schedule = schedule
            };
            return View(model);  // Looks for a view named "ScheduleGenerator.cshtml"
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
      
        [HttpGet]
        public IActionResult InviteEmployee()
        {
            var model = new InviteEmployeeViewModel(); 
            return View(model);
        }

        [HttpPost]
        public IActionResult SendInvitation(InviteEmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("InviteEmployee", model);
            }

            // Generate a unique registration link.
            string registrationLink = GenerateRegistrationLink();

            // Send the registration link to the employee's email.

            // We can display a success message or redirect to a confirmation page.
            // For now, we'll redirect back to the form.
            return RedirectToAction("SignUp", "Home");
        }

        private string GenerateRegistrationLink()
        {

            string token = "unique_token_here";

            // Construct the registration link with the token.
            return Url.Action("SignUp", "Home", new { area = "", token }, Request.Scheme);
        }

        public IActionResult RuleList()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            int userId = int.Parse(userIdClaim.Value);
            var organizationId = Basic_Functions.getEmployeeByID(userId).OrganizationId.Value;
            var rules = Basic_Functions.GetRulesForOrganization(organizationId);
            return View("RuleList", rules);
        }
        public IActionResult CreateRule()
        {
            return View("CreateRule");
        }

        [HttpPost]
        public IActionResult CreateRule(string type, string json, int? employeeId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            int userId = int.Parse(userIdClaim.Value);
            int organizationID = Basic_Functions.getEmployeeByID(userId).OrganizationId.Value;
            RuleBuilder ruleBuilder = new RuleBuilder(organizationID);
            
            if (ModelState.IsValid)
            {
                RuleDecorator ruleDecorator = ruleBuilder.buildSingleRule(type);
                ruleBuilder.SaveRuleToDatabase(ruleDecorator);

                return RedirectToAction("RuleList");
            }
            return View("CreateRule");
        }

        // For editing an existing rule
        public IActionResult EditRule(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            int userId = int.Parse(userIdClaim.Value);
            int organizationID = Basic_Functions.getEmployeeByID(userId).OrganizationId.Value;
            RuleBuilder ruleBuilder = new RuleBuilder(organizationID);

            RuleDecorator ruleDecorator = ruleBuilder.LoadRuleFromDatabase(id);
            if (ruleDecorator == null)
            {
                return NotFound();
            }
            return View("EditRule", ruleDecorator);
        }

        [HttpPost]
        public IActionResult EditRule(RuleDecorator ruleDecorator, int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            int userId = int.Parse(userIdClaim.Value);
            int organizationID = Basic_Functions.getEmployeeByID(userId).OrganizationId.Value;
            RuleBuilder ruleBuilder = new RuleBuilder(organizationID);

            if (ModelState.IsValid)
            {
                ruleBuilder.SaveRuleToDatabase(ruleDecorator, id);
                return RedirectToAction("RuleList");
            }
            return View("EditRule", ruleDecorator);
        }

        // For deleting a rule
        public IActionResult DeleteRule(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            int userId = int.Parse(userIdClaim.Value);
            int organizationID = Basic_Functions.getEmployeeByID(userId).OrganizationId.Value;
            RuleBuilder ruleBuilder = new RuleBuilder(organizationID);

            RuleDecorator ruleDecorator = ruleBuilder.LoadRuleFromDatabase(id);
            if (ruleDecorator == null)
            {
                return NotFound();
            }
            return View("DeleteRule", ruleDecorator);
        }

        [HttpPost]
        public IActionResult DeleteRuleConfirmed(int id)
        {
            Basic_Functions.DeleteRule(id);
            return RedirectToAction("RuleList");
        }
    }
}
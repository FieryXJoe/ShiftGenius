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
using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;
using System.Linq;

namespace ShiftGenius.Controllers
{
    public class ManagerController : Controller
    {
        private readonly ILogger<ManagerController> _logger;
        private readonly AppDbContext _dbContext;
        private List<ShiftGeniusLibDB.Models.TimeOffRequest> _timeOffRequestsData; 

        public ManagerController(ILogger<ManagerController> logger, AppDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
            _timeOffRequestsData = GetTimeOffRequestsFromDatabase(); 
        }

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
            return View(model);
        }

        public static string FormatNullableDate(DateTime? date)
        {
            return date.HasValue ? date.Value.ToShortDateString() : "N/A";
        }

        private List<TimeOffRequestViewModel> GetTimeOffRequestsViewModels()
        {
            return GetTimeOffRequestsFromDatabase()
                .Where(request => request != null)
                .Select(request => new TimeOffRequestViewModel
                {
                    RequestID = request.RequestID,
                    EmployeeID = request.EmployeeID.ToString(),
                    StartDate = FormatNullableDate(request?.StartDate),
                    EndDate = FormatNullableDate(request?.EndDate),
                    Type = request.Type ?? "N/A",
                    Status = request.Status != null ? request.Status : "N/A"
                })
                .ToList();
        }

        public IActionResult ManagerTimeOffRequests()
        {
            // Fetch time-off requests from the database
            var timeOffRequests = GetTimeOffRequestsFromDatabase();

            // Convert the list of TimeOffRequest to TimeOffRequestViewModel
            var timeOffRequestViewModels = timeOffRequests.Select(request => new TimeOffRequestViewModel
            {
                RequestID = request.RequestID,
                EmployeeID = request.EmployeeID.ToString(),
                StartDate = FormatNullableDate(request.StartDate),
                EndDate = FormatNullableDate(request.EndDate),
                Type = request.Type ?? "N/A",
                RequestDate = request.RequestDate,
                Status = request.Status ?? "N/A"
            }).ToList();

            // Create an instance of ManagerTimeOffRequestsViewModel and set its TimeOffRequests property
            var viewModel = new ManagerTimeOffRequestsViewModel
            {
                TimeOffRequests = timeOffRequestViewModels
            };

            return View("ManagerTimeOffRequests", viewModel);
        }

        [HttpPost]
        public IActionResult DeleteTimeOffRequest(int requestId)
        {
            try
            {
                // Retrieve the request from the stored data by ID
                var existingRequest = _timeOffRequestsData.FirstOrDefault(r => r.RequestID == requestId);

                if (existingRequest != null)
                {
                    // Remove the request from the stored data
                    _timeOffRequestsData.Remove(existingRequest);
                    // You can optionally remove it from the database here as well
                    //_dbContext.TimeOffRequests.Remove(existingRequest);
                    //_dbContext.SaveChanges();

                    TempData["SuccessMessage"] = "Time-off request deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Time-off request not found.";
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError($"Error deleting time-off request: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while deleting the time-off request.";
            }

            // Redirect back to the manager's time off requests page
            return RedirectToAction("ManagerTimeOffRequests");
        }

        [HttpPost]
        public IActionResult ProcessTimeOffRequests(List<TimeOffRequestViewModel> timeOffRequests)
        {
            if (timeOffRequests == null || !timeOffRequests.Any())
            {
                // Handle the case where no requests were submitted
                TempData["ErrorMessage"] = "No time-off requests were submitted.";
                return RedirectToAction("ManagerTimeOffRequests");
            }

            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                foreach (var requestViewModel in timeOffRequests)
                {
                    // Retrieve the request from the stored data by ID
                    var existingRequest = _timeOffRequestsData.FirstOrDefault(r => r.RequestID == requestViewModel.RequestID);

                    if (existingRequest != null)
                    {
                        // Update the status based on the selected decision
                        if (requestViewModel.Status == "Approve")
                        {
                            existingRequest.Status = "Approved";
                        }
                        else if (requestViewModel.Status == "Deny")
                        {
                            // Remove the request from the stored data
                            _timeOffRequestsData.Remove(existingRequest);
                            // You can optionally remove it from the database here as well
                            //_dbContext.TimeOffRequests.Remove(existingRequest);
                        }
                    }
                }

                // Save changes to the database
                _dbContext.SaveChanges();

                // Commit the transaction
                transaction.Commit();

                // Redirect back to the manager's time off requests page
                TempData["SuccessMessage"] = "Time-off requests updated successfully!";
                return RedirectToAction("ManagerTimeOffRequests");
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the transaction
                transaction.Rollback();

                // Log the exception for debugging
                _logger.LogError($"Failed to update time-off requests: {ex.Message}");

                TempData["ErrorMessage"] = "Failed to update time-off requests. Please try again later.";
                return RedirectToAction("ManagerTimeOffRequests");
            }
        }

        public List<ShiftGeniusLibDB.Models.TimeOffRequest> GetTimeOffRequestsFromDatabase()
        {
            try
            {
                
                var requests = _dbContext.TimeOffRequests.ToList();

                
                foreach (var request in requests)
                {
                    if (request.Status == null)
                    {
                        // Handle the null status, e.g., set it to a default value
                        request.Status = "N/A";
                    }
                }

                return requests;
            }
            catch (Exception ex)
            {
                
                _logger.LogError($"Error while fetching time-off requests: {ex.Message}");
                return new List<ShiftGeniusLibDB.Models.TimeOffRequest>();
            }
        }

        [Authorize(Policy = "IsManager")]
        public IActionResult ManagerAvailabilityRequests()
        {
            var availabilityRequests = GetAvailabilityRequests(); // Replace with the logic to retrieve availability requests
            return View(availabilityRequests);
        }

        private List<ManagerAvailRequestModel> GetAvailabilityRequests()
        {
            return new List<ManagerAvailRequestModel>
            {
                new ManagerAvailRequestModel { EmployeeID = "001", EmployeeName = "Mike Mike" },
                new ManagerAvailRequestModel { EmployeeID = "002", EmployeeName = "Joe Joe" },
            };
        }


        public IActionResult InviteEmployee()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendInvitation(InviteEmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("InviteEmployee", model);
            }

            // Generate a unique registration link.
            string registrationLink = GenerateUniqueToken();

            // Construct the full registration URL
            var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            var fullRegistrationLink = $"{baseUrl}/Home/SignUp?token={registrationLink}";

            try
            {
                using (var smtpClient = new SmtpClient("smtp.gmail.com"))
                {
                    smtpClient.Port = 587; // Set the SMTP port
                    smtpClient.Credentials = new NetworkCredential("Miketatooine@gmail.com", "atzbmytrfzkhapqe");
                    smtpClient.EnableSsl = true; // Enable SSL if required

                    var mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress("Miketatooine@Gmail.com", "Michael from ShiftGenius");
                    mailMessage.Subject = "Invitation to ShiftGenius";
                    mailMessage.Body = $"You're invited to join ShiftGenius! Click on the following link to register: <a href='{fullRegistrationLink}'>{fullRegistrationLink}</a>";
                    mailMessage.IsBodyHtml = true;
                    mailMessage.To.Add(model.EmailAddress);

                    // Send the email
                    await smtpClient.SendMailAsync(mailMessage);
                }

                // Email sent successfully
                TempData["SuccessMessage"] = "Invitation email sent successfully!";
                return RedirectToAction("InviteEmployee", "Manager");
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during email sending
                TempData["ErrorMessage"] = "Failed to send the invitation email.";
                // Log the exception for debugging
                _logger.LogError($"Failed to send the invitation email: {ex.Message}");
                return RedirectToAction("Error");
            }
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
        public IActionResult DeleteRuleConfirmed(int ruleId)
        {
            Basic_Functions.DeleteRule(ruleId);
            return RedirectToAction("RuleList");
        }

        public IActionResult AddRule()
        {
            return View("AddRule");
        }
        public IActionResult AddMinHours()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            int userId = int.Parse(userIdClaim.Value);
            ViewBag.Employees = Basic_Functions.GetEmployeesInOrganization(Basic_Functions.getEmployeeByID(userId).OrganizationId.Value);
            return View();
        }

        [HttpPost]
        public IActionResult AddMinHours(int employee, int minHours, DateTime? startDate, DateTime? endDate)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);
            int organizationId = Basic_Functions.getEmployeeByID(userId).OrganizationId.Value;

            ScheduleRule newRule = new ScheduleRule
            {
                EmployeeId = employee,
                StartTime = startDate,
                EndTime = endDate,
                OrganizationId = organizationId,
                CreatedBy = userId,
                DateCreated = DateTime.Now,
                Approved = true
            };

            MinHoursDecorator minHoursRule = new MinHoursDecorator(Basic_Functions.getEmployeeByID(employee), minHours, new Schedule(organizationId));

            newRule.Rule = minHoursRule.EncodeJSON();

            Basic_Functions.AddRule(newRule);

            return RedirectToAction("RuleList");
        }
        public IActionResult ViewRule(int id)
        {
            ScheduleRule rule = Basic_Functions.GetRuleById(id);

            if (rule == null)
            {
                return NotFound();
            }

            return View(rule);
        }
        public IActionResult AddMaxHours()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            int userId = int.Parse(userIdClaim.Value);
            ViewBag.Employees = Basic_Functions.GetEmployeesInOrganization(Basic_Functions.getEmployeeByID(userId).OrganizationId.Value);
            return View();
        }

        [HttpPost]
        public IActionResult AddMaxHours(int employee, int maxHours, DateTime? startDate, DateTime? endDate)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);
            int organizationId = Basic_Functions.getEmployeeByID(userId).OrganizationId.Value;

            ScheduleRule newRule = new ScheduleRule
            {
                EmployeeId = employee,
                StartTime = startDate,
                EndTime = endDate,
                OrganizationId = organizationId,
                CreatedBy = userId,
                DateCreated = DateTime.Now,
                Approved = true
            };

            MaxHoursDecorator maxHoursRule = new MaxHoursDecorator(Basic_Functions.getEmployeeByID(employee), maxHours, new Schedule(organizationId));

            newRule.Rule = maxHoursRule.EncodeJSON();

            Basic_Functions.AddRule(newRule);

            return RedirectToAction("RuleList");
        }

        public IActionResult AddOperatingHours()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddOperatingHours(TimeSpan startTime, TimeSpan endTime, DateTime? startDate, DateTime? endDate)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);
            int organizationId = Basic_Functions.getEmployeeByID(userId).OrganizationId.Value;

            OperatingHoursDecorator decorator = new OperatingHoursDecorator(startTime, endTime, new Schedule(organizationId), organizationId);
            string json = decorator.EncodeJSON();

            ScheduleRule newRule = new ScheduleRule
            {
                EmployeeId = null,
                StartTime = startDate,
                EndTime = endDate,
                Rule = json,
                OrganizationId = organizationId,
                CreatedBy = userId,
                DateCreated = DateTime.Now,
                Approved = true
            };

            Basic_Functions.AddRule(newRule);

            return RedirectToAction("RuleList");
        }
        public IActionResult AddMinEmployees()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddMinEmployees(int numEmployees, TimeSpan startTime, TimeSpan endTime, DateTime? startDate, DateTime? endDate)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);
            int organizationId = Basic_Functions.getEmployeeByID(userId).OrganizationId.Value;


            MinEmployeesDecorator decorator = new MinEmployeesDecorator(numEmployees, new Schedule(organizationId), startTime, endTime);

            string json = decorator.EncodeJSON();

            ScheduleRule newRule = new ScheduleRule
            {
                EmployeeId = null,
                StartTime = startDate,
                EndTime = endDate,
                Rule = json,
                OrganizationId = organizationId,
                CreatedBy = userId,
                DateCreated = DateTime.Now,
                Approved = true
            };

            Basic_Functions.AddRule(newRule);

            return RedirectToAction("RuleList");
        }

        [HttpPost]
        public IActionResult SaveSchedule(string SerializedSchedule)
        {
            Schedule schedule = JsonConvert.DeserializeObject<Schedule>(SerializedSchedule);

            // Call the built-in save function of the Schedule object
            schedule.SaveChanges();

            // Redirect or return a view
            return RedirectToAction("Index");
        }

        private string GenerateUniqueToken()
        {
            // Generate a unique token using a GUID
            Guid uniqueGuid = Guid.NewGuid();
            string uniqueToken = uniqueGuid.ToString();
            return uniqueToken;
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftGenius.Models;
using System.Net;
using System.Net.Mail;
using System.Linq;

namespace ShiftGenius.Controllers
{
    public class ManagerController : Controller
    {
        private readonly ILogger<ManagerController> _logger;
        private readonly AppDbContext _dbContext;

        public ManagerController(ILogger<ManagerController> logger, AppDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [Authorize(Policy = "IsManager")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ScheduleGenerator()
        {
            return View();
        }

        public IActionResult RuleList()
        {
            return View();
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

        [Authorize(Policy = "IsManager")]
        public IActionResult ManagerTimeOffRequests()
        {
            // Fetch time-off requests from the database
            var timeOffRequests = GetTimeOffRequestsFromDatabase();

            return View("ManagerTimeOffRequests", timeOffRequests);
        }


        [HttpPost]
        public IActionResult ProcessTimeOffRequests(List<TimeOffRequestViewModel> timeOffRequests)
        {
            if (timeOffRequests != null && timeOffRequests.Any())
            {
                foreach (var requestViewModel in timeOffRequests)
                {
                    // Retrieve the request from the database by ID
                    var existingRequest = _dbContext.TimeOffRequests.FirstOrDefault(r => r.RequestID == requestViewModel.RequestID);

                    if (existingRequest != null)
                    {
                        // Update the status based on the selected decision
                        if (requestViewModel.Status == "Approve")
                        {
                            existingRequest.Status = "Approved";
                        }
                        else if (requestViewModel.Status == "Deny")
                        {
                            existingRequest.Status = "Denied";
                        }

                        // Save changes to the database
                        _dbContext.SaveChanges();
                    }
                }

                // Redirect back to the manager's time off requests page
                return RedirectToAction("ManagerTimeOffRequests");
            }

            // Handle the case where no requests were submitted
            return RedirectToAction("ManagerTimeOffRequests");
        }

        public List<ShiftGeniusLibDB.Models.TimeOffRequest> GetTimeOffRequestsFromDatabase()
        {
            try
            {
                // Fetch time-off requests from the database
                var requests = _dbContext.TimeOffRequests.ToList();

                // Handle null values for specific properties, if needed
                foreach (var request in requests)
                {
                    if (request.Status == null)
                    {
                        // Handle the null status, e.g., set it to a default value
                        request.Status = "N/A";
                    }

                    // Handle other properties with potential null values as needed
                }

                return requests;
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log it) or return an empty list
                // You might want to log the exception for debugging purposes
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

        private string GenerateUniqueToken()
        {
            // Generate a unique token using a GUID
            Guid uniqueGuid = Guid.NewGuid();
            string uniqueToken = uniqueGuid.ToString();
            return uniqueToken;
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftGenius.Models;
using System.Net;
using System.Net.Mail;


namespace ShiftGenius.Controllers
{
    public class ManagerController : Controller
    {
        private readonly ILogger<ManagerController> _logger;

        public ManagerController(ILogger<ManagerController> logger)
        {
            _logger = logger;
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
                return RedirectToAction("Index", "Manager");
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
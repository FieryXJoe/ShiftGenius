using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SendGrid;
using SendGrid.Helpers.Mail;
using ShiftGenius.Models;
using System.Collections.Generic;
using System.Net.Mail;

namespace ShiftGenius.Controllers
{
    public class ManagerController : Controller
    {
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

            // Send the registration link to the employee's email using SendGrid.
            var sendGridApiKey = "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"; 
            var client = new SendGridClient(sendGridApiKey);

            var msg = new SendGridMessage()
            {
                From = new EmailAddress("ml102753@ShiftGenius.com", "Michael Lopez"),
                Subject = "Invitation to ShiftGenius",
                PlainTextContent = $"You're invited to join ShiftGenius! Click on the following link to register: {registrationLink}",
                HtmlContent = $"<p>You're invited to join ShiftGenius! Click on the following link to register: <a href='{registrationLink}'>{registrationLink}</a></p>"
            };

            msg.AddTo(model.EmailAddress);

            var response = await client.SendEmailAsync(msg);

            // Check if the email was sent successfully (you can add error handling here)
            if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                // Handle the case where the email failed to send.
                // You can log the error or display a message to the user.
                TempData["ErrorMessage"] = "Failed to send the invitation email.";
                return RedirectToAction("Error");
            }

            // Email sent successfully
            TempData["SuccessMessage"] = "Invitation email sent successfully!";
            return RedirectToAction("SignUp", "Home");
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
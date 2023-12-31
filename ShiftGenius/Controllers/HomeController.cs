﻿using Microsoft.AspNetCore.Mvc;
using ShiftGenius.Models;
using System.Diagnostics;

using ShiftGeniusLibDB;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace ShiftGenius.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (User.HasClaim("IsManager", "true"))
            {
                return RedirectToAction("Index", "Manager");
            }
            else if (User.HasClaim("IsManager", "false"))
            {
                return RedirectToAction("Index", "Employee");
            }
            else
            {
                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int userId = Basic_Functions.checkLoginCredentials(model.Email, model.Password);

            if (userId == -1)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            var userClaims = new List<Claim>
            {
                // Add a claim to store userID
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, Basic_Functions.getEmployeeNameByID(userId))
            };

            if (Basic_Functions.isManager(userId))
            {
                userClaims.Add(new Claim("IsManager", "true"));
                var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties();
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                return RedirectToAction("Index", "Manager");
            }
            else
            {
                userClaims.Add(new Claim("IsManager", "false"));
                var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties();
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                return RedirectToAction("Index", "Employee");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        // POST: Home/SignUp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool result = Basic_Functions.AddEmployee(model.Name, model.Email, model.Password);

            if (result)
            {
                return RedirectToAction("Index", "Employee");
            }
            else
            {
                ModelState.AddModelError("", "There was an error registering the user. Please try again.");
                return View(model);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet("SignUp/{token?}")]
        public IActionResult SignUp(string token = null)
        {
            var viewModel = new SignUpViewModel { Token = token };
            return View(viewModel);
        }

    }
}
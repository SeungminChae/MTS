using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using MarysToyStore.DataAccess.Data;
using MarysToyStore.DataAccess.Models;
using MarysToyStore.Services;
using MarysToyStore.Models;
using Microsoft.Extensions.Logging;

namespace MarysToyStore.Controllers
{
    [Authorize]
    [Route("")]
    public class AccountController : Controller
    {
        private readonly DataService _dataService;
        private readonly UspsService _uspsService;

        private readonly ILogger<AccountController> _log;

        public AccountController(DataContext dataContext, UspsService uspsService, ILogger<AccountController> log)
        {
            // Instantiate an instance of the data service.
            _dataService = new DataService(dataContext);
            _uspsService = uspsService;
            _log = log;
        }

        [AllowAnonymous]
        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            User existingUser = _dataService.GetUser(registerViewModel.EmailAddress);
            if (existingUser != null)
            {
                // Set email address already in use error message.
                ModelState.AddModelError("Error", "An account already exists with that email address.");

                return View();
            }

            PasswordHasher<string> passwordHasher = new PasswordHasher<string>();

            User user = new User()
            {
                FirstName = registerViewModel.FirstName,
                LastName = registerViewModel.LastName,
                EmailAddress = registerViewModel.EmailAddress,
                PasswordHash = passwordHasher.HashPassword(null, registerViewModel.Password)
            };

            _dataService.AddUser(user);

            _log.LogInformation($"The {registerViewModel.EmailAddress} user has registered.");

            return RedirectToAction(nameof(Login));
        }

        [AllowAnonymous]
        [HttpGet("sign-in")]
        public IActionResult Login(string returnUrl)
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost("sign-in")]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            User user = _dataService.GetUser(loginViewModel.EmailAddress);

            if (user == null)
            {
                // Set email address not registered error message.
                ModelState.AddModelError("Error", "An account does not exist with that email address.");

                return View();
            }

            PasswordHasher<string> passwordHasher = new PasswordHasher<string>();
            PasswordVerificationResult passwordVerificationResult =
                passwordHasher.VerifyHashedPassword(null, user.PasswordHash, loginViewModel.Password);

            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                // Set invalid password error message.
                ModelState.AddModelError("Error", "Invalid password.");
                _log.LogInformation($"Invalid login for {loginViewModel.EmailAddress} ({user.Id}).");

                return View();
            }

            _log.LogInformation($"User logged in: {loginViewModel.EmailAddress} ({user.Id}).");

            // Add the user's ID (NameIdentifier), first name and role
            // to the claims that will be put in the cookie.
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties { };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            if (string.IsNullOrEmpty(returnUrl))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return Redirect(returnUrl);
            }
        }

        [Route("sign-out")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet("access-denied")]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet("profile")]
        public IActionResult Profile()
        {
            // Get currently logged in user ID from the auth cookie.
            int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // Get user.
            User u = _dataService.GetUser(userId);

            return View(u);
        }

        [HttpGet("edit-profile")]
        public IActionResult EditProfile()
        {
            // Get user id.
            int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // Get user.
            User u = _dataService.GetUser(userId);

            // Populate view model.
            EditProfileViewModel vm = new EditProfileViewModel()
            {
                EmailAddress = u.EmailAddress,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Address = u.Address,
                Address2 = u.Address2,
                City = u.City,
                State = u.State,
                Zip = u.Zip
            };

            return View(vm);
        }

        [HttpPost("edit-profile")]
        public IActionResult EditProfile(EditProfileViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            // Get current user.
            User current = _dataService.GetUser(Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));

            PasswordHasher<string> hasher = new PasswordHasher<string>();

            // Confirm password.
            if (hasher.VerifyHashedPassword(null, current.PasswordHash, vm.OldPassword) == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("OldPassword", "Your password is incorrect.");

                return View(vm);
            }

            // Validate address.
            bool addressVerified = _uspsService.ValidateAddress(vm.Address, vm.Address2, vm.City, vm.State, vm.Zip).Result;
            if (!addressVerified)
            {
                ModelState.AddModelError("Address", "The address you entered is invalid.");
                return View(vm);
            }

            // Set user fields.
            current.FirstName = vm.FirstName;
            current.LastName = vm.LastName;
            current.EmailAddress = vm.EmailAddress;
            current.Address = vm.Address;
            current.Address2 = vm.Address2;
            current.City = vm.City;
            current.State = vm.State;
            current.Zip = vm.Zip;

            // Check if we should be updating the password.
            if (!string.IsNullOrEmpty(vm.NewPassword))
            {
                // Hash password.
                current.PasswordHash = hasher.HashPassword(null, vm.NewPassword);
            }

            // Update.
            _dataService.UpdateUser(current);

            return RedirectToAction(nameof(Profile));
        }
    }
}
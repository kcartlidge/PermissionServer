using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using PermissionServer;
using SampleSite.Models.RequestModels;

namespace SampleSite.Controllers
{
    public class AccountController : Controller
    {
        private readonly PermissionServer.PermissionServer permissionServer;

        public AccountController(PermissionServer.PermissionServer permissionServer)
        {
            this.permissionServer = permissionServer;
        }

        [HttpGet]
        [Route("/login")]
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        [Route("/send-confirmation")]
        public async Task<IActionResult> SendConfirmation(LoginRequest model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Start the confirmation process.
                    var confirmUrl = $"{Request.Scheme}://{Request.Host}/{nameof(Confirm)}";
                    await permissionServer.StartConfirmation(model.EmailAddress, confirmUrl);

                    // Always sign out if successfully starting a login attempt.
                    await Request.HttpContext.SignOutAsync();
                    await HttpContext.SignOutAsync();
                    return RedirectToAction(nameof(HomeController.Index), "home");
                }
                catch
                {
                    ModelState.AddModelError("", "An error occurred; please try again later.");
                }
            }
            return View(nameof(Login));
        }

        [HttpGet]
        [Route("/confirm")]
        public async Task<IActionResult> Confirm()
        {
            return View();
        }

        [HttpPost]
        [Route("/confirm")]
        public async Task<IActionResult> ConfirmPost(ConfirmationRequest model)
        {
            if (ModelState.IsValid)
            {
                var status = permissionServer.CompleteConfirmation(model.EmailAddress, model.ConfirmationCode);
                if (status == ConfirmationStatus.Okay)
                {
                    // At this point the site's own systems should be checked for
                    // a matching account now the email address has been confirmed.
                    // Then either continue to the sign-in as below (updating the
                    // claims accordingly), or redirect to an account creation form,
                    // or even silently create an account in the background.

                    // Sign in.
                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    identity.AddClaim(new Claim(ClaimTypes.Name, "Example"));
                    identity.AddClaim(new Claim("ID", "1"));
                    identity.AddClaim(new Claim("IsAdmin", "Y"));
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return RedirectToAction("Dashboard", "Home");
                }
                else
                {
                    ModelState.AddModelError(
                        nameof(model.ConfirmationCode),
                        "The token is not valid, has expired, or has already been used - please recheck your email.");
                }
            }
            return View(nameof(Confirm));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}

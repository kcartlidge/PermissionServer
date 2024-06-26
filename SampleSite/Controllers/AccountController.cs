using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
        public IActionResult Login() => View();

        [HttpPost]
        [Route("/send-confirmation")]
        public async Task<IActionResult> SendConfirmation(LoginRequest model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // This may be null in which case the extra factor is useless.
                    // The intellisense on the StartConfirmation parameter has suggestions.
                    var context = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";

                    // Start the confirmation process.
                    var confirmUrl = $"{Request.Scheme}://{Request.Host}/{nameof(Confirm)}";
                    var added = await permissionServer.StartConfirmation(model.EmailAddress, confirmUrl, context);
                    if (added)
                    {
                        // Always sign out if successfully starting a login attempt.
                        await Request.HttpContext.SignOutAsync();
                        await HttpContext.SignOutAsync();
                        return RedirectToAction(nameof(HomeController.Index), "home");
                    }
                    ModelState.AddModelError("", "Either the email address is invalid or there have been too many attempts (please wait a while and try again).");
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
        public IActionResult Confirm() => View();

        [HttpPost]
        [Route("/confirm")]
        public async Task<IActionResult> ConfirmPost(ConfirmationRequest model)
        {
            if (ModelState.IsValid)
            {
                // See the comments in SendConfirmation.
                var context = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";

                var matched = permissionServer.CompleteConfirmation(model.EmailAddress, model.ConfirmationCode, context);
                if (matched)
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
                        "The confirmation code is not valid, has expired, or has already been used - please recheck your email.");
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

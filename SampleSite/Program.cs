using Microsoft.AspNetCore.Authentication.Cookies;
using PermissionServer;
using SampleSite.Controllers;
using static PermissionServer.PermissionServerOptions;

namespace SampleSite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure cookie-based authentication.
            builder.Services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(o =>
                {
                    o.Cookie.HttpOnly = true;
                    o.Cookie.SameSite = SameSiteMode.Strict;
                    o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    o.ExpireTimeSpan = TimeSpan.FromMinutes(120);
                    o.SlidingExpiration = true;
                    o.LoginPath = $"/{nameof(AccountController.Login)}";
                });

            builder.Services.AddControllersWithViews();

            // Permission Server (passwordless logins via email).
            var psOpts = new PermissionServerOptions
            {
                Tokens = new TokenOptions
                {
                    Length = 8,
                    LifetimeMinutes = 15,
                    SingleUse = true,
                    MaximumActivePerKey = 5,
                },
                Emails = new EmailOptions
                {
                    // Example dev-suitable online email system.
                    Hostname = "smtp.ethereal.email",
                    Port = 587,
                    StartTLS = true,
                    Username = "[FILL ME IN]",
                    Password = "[FILL ME IN]",
                    Sender = "Sample App <[FILL ME IN]>",
                },
            };
            builder.Services.AddPermissionServer(psOpts);

            var app = builder.Build();
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();
            app.UseRouting();

            // Activate the cookie-based authentication.
            app.UseAuthentication();

            app.UseAuthorization();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.Run();
        }
    }
}

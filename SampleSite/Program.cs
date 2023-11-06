using Microsoft.AspNetCore.Authentication.Cookies;

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
                });

            builder.Services.AddControllersWithViews();

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

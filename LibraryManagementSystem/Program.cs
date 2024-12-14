using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Serilog;

namespace LibraryManagementSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console() // Optional: Log to console
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day) // Log to file with daily rolling
                .CreateLogger();

            try
            {
                Log.Information("Starting up the application");

                var builder = WebApplication.CreateBuilder(args);

                // Configure Serilog for the application
                builder.Host.UseSerilog();

                var ConnectionString = builder.Configuration.GetConnectionString("dbcs");

                // Adding services to the container.
                builder.Services.AddControllersWithViews();

                // Registering LibraryManagementContext
                builder.Services.AddDbContext<LibraryManagementContext>(options =>
                    options.UseSqlServer(ConnectionString, sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()));

                // Add Session Services
                builder.Services.AddSession();

                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                }).AddCookie();

                builder.Services.AddAuthorization(options =>
                {
                    options.AddPolicy("LibrarianPolicy", policy => policy.RequireRole("Librarian"));
                    options.AddPolicy("MemberPolicy", policy => policy.RequireRole("Member"));
                });

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Home/Error");
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseStaticFiles();
                app.UseSession();

                app.UseRouting();
                app.UseAuthentication(); // The authentication is important
                app.UseAuthorization();  // Add authorization middleware

                app.UseSerilogRequestLogging(); // Log all HTTP requests

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}

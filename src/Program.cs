using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using src.Data;
using src.Services;

namespace src
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Create the application builder.
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Retrieve the database connection string from configuration.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            // Register the Entity Framework Core database context using PostgreSQL.
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

            // Add detailed database exception information during development.
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // Configure ASP.NET Core Identity using the application database context.
            // Account confirmation is not required before signing in.
            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // Add Razor Pages services.
            builder.Services.AddRazorPages();

            // Add requires services.
            builder.Services.AddScoped<WatchlistService>();

            builder.Services.AddHttpClient<TmdbService>();

            // Build the application.
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                // Enable the migrations endpoint during development.
                app.UseMigrationsEndPoint();
            }
            else
            {
                // Use the general error page outside of development.
                app.UseExceptionHandler("/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Redirect HTTP requests to HTTPS.
            app.UseHttpsRedirection();

            // Enable routing.
            app.UseRouting();

            // Enable authorization.
            app.UseAuthorization();

            // Map static assets.
            app.MapStaticAssets();

            // Map Razor Pages and their associated static assets.
            app.MapRazorPages()
               .WithStaticAssets();

            // Start the application.
            app.Run();
        }
    }
}
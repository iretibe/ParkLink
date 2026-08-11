using Microsoft.AspNetCore.Identity;
using ParkLink.Identity.Enums;
using ParkLink.Identity.Models;

namespace ParkLink.Identity.Data
{
    public static class IdentitySeed
    {
        public static async Task SeedAsync(
            IServiceProvider services, IConfiguration configuration)
        {
            var roleManager =
                services.GetRequiredService<RoleManager<IdentityRole>>();

            var userManager =
                services.GetRequiredService<UserManager<ApplicationUser>>();

            await SeedRolesAsync(roleManager);

            await SeedAdminAsync(userManager, configuration);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            var roles = new[]
            {
                "SuperAdmin",
                "Admin",
                "ParkingOperator",
                "ParkingManager",
                "SupportAgent",
                "FinanceOfficer",
                "Driver",
                "User"
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result =
                        await roleManager.CreateAsync(new IdentityRole(role));

                    if (!result.Succeeded)
                    {
                        throw new InvalidOperationException(
                            $"Failed to create role '{role}': " +
                            string.Join(
                                ", ",
                                result.Errors.Select(
                                    e => e.Description)));
                    }
                }
            }
        }

        private static async Task SeedAdminAsync(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            var email = configuration["SeedAdmin:Email"];

            var password = configuration["SeedAdmin:Password"];

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "SeedAdmin:Email and SeedAdmin:Password " +
                    "must be configured.");
            }

            var admin = await userManager.FindByEmailAsync(email);

            if (admin is null)
            {
                admin = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,

                    FirstName = "ParkLink",
                    LastName = "Administrator",

                    PreferredLanguage = "en",
                    CountryCode = "GH",
                    TimeZoneId = "Africa/Accra",

                    IsDriver = false,
                    IsActive = true,

                    VerificationStatus = DriverVerificationStatus.Verified,

                    CreatedAtUtc = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(admin, password);

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        "Failed to create seed admin: " +
                        string.Join(
                            ", ",
                            result.Errors.Select(
                                e => e.Description)));
                }
            }

            if (!await userManager.IsInRoleAsync(admin, "SuperAdmin"))
            {
                await userManager.AddToRoleAsync(admin, "SuperAdmin");
            }
        }
    }
}

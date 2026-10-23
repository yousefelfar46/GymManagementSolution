using GymManagement.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;

namespace GymManagement.DAL.DATA.DataSeeding
{
    public class IdentityDataSeeding
    {
        public static async Task SeedIdentityDataAsync(
            RoleManager<IdentityRole> roleManager,
             UserManager<ApplicationUser> userManager, ILogger logger,
             CancellationToken ct = default)
        {
            try
            {
                var hasUsers = await userManager.Users.AnyAsync(ct);
                var hasRoles = await roleManager.Roles.AnyAsync(ct);

                if (hasUsers && hasRoles) return;


                if (!hasRoles)
                {
                    var roles = new List<IdentityRole>
                {
                    new IdentityRole("SuperAdmin"),
                    new IdentityRole("Admin")
                };
                    foreach (var role in roles)
                    {
                        var result = await roleManager.CreateAsync(role);
                        if (!result.Succeeded)
                        {
                            logger.LogError($"Failed to create role {role.Name}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                        }
                    }
                }

                if (!hasUsers)
                {
                    var superAdmin = new ApplicationUser
                    {

                        FirstName = "yousef",
                        LastName = "elfar",
                        UserName = "YousefElfar",
                        Email = "yousef@gmail.com",
                        PhoneNumber = "01000000000"

                    };
                    var admin = new ApplicationUser
                    {
                        FirstName = "ahmed",
                        LastName = "mohammed",
                        UserName = "AhmedMohammed",
                        Email = "ahmed@example.com",
                        PhoneNumber = "01000000001"
                    };
                    var superAdminRes = await userManager.CreateAsync(superAdmin, "Password123!");
                    if (superAdminRes.Succeeded)
                    {
                        await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
                    }


                    var adminResult = await userManager.CreateAsync(admin, "Password123!");
                    if (adminResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, "Admin");
                    }


                    logger.LogInformation("Seeded initial users and roles.");
                }


                return;
            }
            catch (Exception ex)
            {

                logger.LogError(ex, "Identity Seeding Failed");
                return;
            }

        }
    }
}

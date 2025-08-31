using Microsoft.AspNetCore.Identity;
using Myblog.Models;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public static class DataSeeder
{
    public static async Task SeedRolesAndUsers(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roles = { "Admin", "SuperAdmin" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Create a SuperAdmin user
        var superAdminEmail = "superadmin@example.com";
        if (await userManager.FindByEmailAsync(superAdminEmail) == null)
        {
            var user = new ApplicationUser { UserName = superAdminEmail, Email = superAdminEmail, FullName = "Super Admin" };
            var result = await userManager.CreateAsync(user, "SuperAdmin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "SuperAdmin");
            }
        }
    }
}

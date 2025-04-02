using AboutMeApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace AboutMeApp.Infrastructure;

public static class DbSeeder
{
    public static async Task SeedRolesAndUsers(UserManager<User> userManager, RoleManager<Role> roleManager)
    {
        string adminEmail = "admin@gmail.com";
        string adminPassword = "Admin123+";

        // Roller yoksa oluştur
        string[] roles = { "admin", "user" };
        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new Role
                {
                    Name = roleName,
                    NormalizedName = roleName.ToUpper()
                };
                await roleManager.CreateAsync(role);
            }
        }

        // Admin kullanıcı var mı kontrol et
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new User
            {
                UserName = adminEmail,
                Name = "Admin",
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                // Kullanıcıya roller ekleniyor
                await userManager.AddToRolesAsync(adminUser, roles);
                Console.WriteLine("Admin user created successfully!");
            }
            else
            {
                Console.WriteLine("Admin user creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            Console.WriteLine("Admin user already exists.");
        }
    }

}

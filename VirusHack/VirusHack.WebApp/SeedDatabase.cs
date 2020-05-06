using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirusHack.WebApp.Models;

namespace VirusHack.WebApp
{
    public static class SeedDatabase
    {
        public static async Task CreateRoles(IServiceProvider provider, IConfiguration configuration)
        {
            var userManager = provider.GetRequiredService<UserManager<User>>();
            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            var roles = configuration.GetSection("Roles").Get<string[]>();

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }

            var superUser = new User
            {
                FirstName = "Tester",
                LastName = "Testov",
                Email = "test@test.ru",
                UserName = "test@test.ru",
                UserStatus = UserStatus.Student
            };

            string password = "1234567890";
            var user = await userManager.FindByEmailAsync(superUser.Email);

            if (user == null)
            {
                var superUserResult = await userManager.CreateAsync(superUser, password);
                //if (superUserResult.Succeeded)
                //{
                //    await userManager.AddToRoleAsync(superUser, "Manager");
                //}
            }
        }
    }
}

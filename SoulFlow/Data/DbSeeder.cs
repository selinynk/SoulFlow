using Microsoft.AspNetCore.Identity;

using SoulFlow.Models;



namespace SoulFlow.Data

{

    public static class DbSeeder

    {

        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)

        {

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();



            string[] roleNames = { "Admin", "Member" };

            foreach (var roleName in roleNames)

            {

                if (!await roleManager.RoleExistsAsync(roleName))

                {

                    await roleManager.CreateAsync(new IdentityRole(roleName));

                }

            }





            var adminEmail = "admin_soulflow@gmail.com";



            var adminUser = await userManager.FindByEmailAsync(adminEmail);



            if (adminUser == null)

            {

                var newAdmin = new AppUser

                {



                    UserName = "G231210057",

                    Email = adminEmail,

                    EmailConfirmed = true,

                    Bio = "Sistem Yöneticisi",

                    ProfileImage = ""

                };





                var createPowerUser = await userManager.CreateAsync(newAdmin, "Sfa.123!");



                if (createPowerUser.Succeeded)

                {



                    await userManager.AddToRoleAsync(newAdmin, "Admin");

                }

                else

                {



                    var errors = string.Join(", ", createPowerUser.Errors.Select(e => e.Description));

                    throw new Exception("Admin oluşturulamadı! Hata: " + errors);

                }

            }

        }

    }

}
using HELMo_bilite.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HELMo_bilite.Data
{
    public static class DataInitializer
    {
        private static readonly string[] Roles = new string[] { "Admin", "Client", "Driver", "Dispatcher" };

        private static readonly string DefaultPassword = "74187418Aa&";

        private static readonly List<(string email, string role)> Users = new List<(string email, string role)>
        {
            ("admin@gmail.com", "Admin"),
            ("dispatcher1@gmail.com", "Dispatcher"),
            ("dispatcher2@gmail.com", "Dispatcher"),
            ("driver1@gmail.com", "Driver"),
            ("driver2@gmail.com", "Driver"),
            ("driver3@gmail.com", "Driver"),
            ("driver4@gmail.com", "Driver"),
            ("driver5@gmail.com", "Driver"),
            ("client1@gmail.com", "Client"),
            ("client2@gmail.com", "Client"),
            ("client3@gmail.com", "Client"),
            ("client4@gmail.com", "Client"),
            ("client5@gmail.com", "Client")
        };

        public static async Task Initialize(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, HELMoBiliteDbContext context)
        {
            foreach (var role in Roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            foreach (var (email, role) in Users)
            {
                if (await userManager.FindByEmailAsync(email) == null)
                {
                    User user = new User
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true
                    };

                    IdentityResult result = await userManager.CreateAsync(user, DefaultPassword);

                    if (result.Succeeded)
                    {
                        // Save user to database to generate User ID
                        await context.SaveChangesAsync();

                        IdentityResult roleResult = await userManager.AddToRoleAsync(user, role);

                        if (roleResult.Succeeded)
                        {
                            // Fetch the user again after it has been saved to the database to get the User ID
                            user = await userManager.FindByEmailAsync(email);

                            switch (role)
                            {
                                case "Admin":
                                    context.Admins.Add(new Admin { UserId = user.Id });
                                    break;
                                case "Client":
                                    context.Clients.Add(new Client
                                    {
                                        UserId = user.Id,
                                        CompanyName = "Company " + email,
                                        Street = "Street",
                                        Number = 1,
                                        City = "City",
                                        PostalCode = 1000,
                                        Country = "Country"
                                    });
                                    break;
                                case "Driver":
                                    context.Drivers.Add(new Driver
                                    {
                                        UserId = user.Id,
                                        Matricule = "Matricule " + email,
                                        LastName = "Last Name",
                                        FirstName = "First Name",
                                        LicenseType = LicenseType.C
                                    });
                                    break;
                                case "Dispatcher":
                                    context.Dispatchers.Add(new Dispatcher
                                    {
                                        UserId = user.Id,
                                        Matricule = "Matricule " + email,
                                        LastName = "Last Name",
                                        FirstName = "First Name",
                                        EducationLevel = EducationLevel.CESS
                                    });
                                    break;
                            }
                        }
                    }
                }
            }
            await context.SaveChangesAsync();
        }
    }
}

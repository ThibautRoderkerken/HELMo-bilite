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

        private static readonly List<Truck> Trucks = new List<Truck>
        {
            new Truck { LicensePlate = "1-CAM-001", Brand = Truck.TruckBrand.DAF, Model = "XF 480", Type = Truck.TruckType.C, Payload = 20, ImageName = "trucks/_camion01.png" },
            new Truck { LicensePlate = "1-CAM-002", Brand = Truck.TruckBrand.Mercedes, Model = "XF 481", Type = Truck.TruckType.CE, Payload = 40, ImageName = "trucks/_camion02.png" },
            new Truck { LicensePlate = "1-CAM-003", Brand = Truck.TruckBrand.Mack, Model = "XF 482", Type = Truck.TruckType.C, Payload = 20, ImageName = "trucks/_camion03.png" },
            new Truck { LicensePlate = "1-CAM-004", Brand = Truck.TruckBrand.MAN, Model = "XF 483", Type = Truck.TruckType.CE, Payload = 40, ImageName = "trucks/_camion04.png" },
            new Truck { LicensePlate = "1-CAM-005", Brand = Truck.TruckBrand.Peterbilt, Model = "XF 484", Type = Truck.TruckType.C, Payload = 20, ImageName = "trucks/_camion05.png" },
            new Truck { LicensePlate = "1-CAM-006", Brand = Truck.TruckBrand.Kenworth, Model = "XF 485", Type = Truck.TruckType.CE, Payload = 40, ImageName = "trucks/_camion06.png" },
            new Truck { LicensePlate = "1-CAM-007", Brand = Truck.TruckBrand.Scania, Model = "XF 486", Type = Truck.TruckType.C, Payload = 20, ImageName = "trucks/_camion07.png" },
            new Truck { LicensePlate = "1-CAM-008", Brand = Truck.TruckBrand.Iveco, Model = "XF 487", Type = Truck.TruckType.CE, Payload = 40, ImageName = "trucks/_camion08.png" },
            new Truck { LicensePlate = "1-CAM-009", Brand = Truck.TruckBrand.Renault, Model = "XF 488", Type = Truck.TruckType.C, Payload = 20, ImageName = "trucks/_camion09.png" },
            new Truck { LicensePlate = "1-CAM-010", Brand = Truck.TruckBrand.Volvo, Model = "XF 489", Type = Truck.TruckType.CE, Payload = 40, ImageName = "trucks/_camion10.png" },
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

            int i = 0;
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
                                        CompanyName = "Company " + i,
                                        Street = "Street",
                                        Number = 1 + i,
                                        City = "City",
                                        PostalCode = 1000,
                                        Country = "Country"
                                    });
                                    break;
                                case "Driver":
                                    context.Drivers.Add(new Driver
                                    {
                                        UserId = user.Id,
                                        Matricule = "Q2100 " + i,
                                        LastName = "Last Name" + i,
                                        FirstName = "First Name" + i,
                                        LicenseType = i % 2 == 0 ? LicenseType.CE : LicenseType.C,
                                    });
                                    break;
                                case "Dispatcher":
                                    context.Dispatchers.Add(new Dispatcher
                                    {
                                        UserId = user.Id,
                                        Matricule = "Q2100 " + i,
                                        LastName = "Last Name" + i,
                                        FirstName = "First Name" + i,
                                        EducationLevel = EducationLevel.CESS
                                    });
                                    break;
                            }
                        }
                    }
                }
                i++;
            }

            // Parcourir les camions et les ajouter à la base de données si ils n'y sont pas déjà
            foreach (var truck in Trucks)
            {
                // Récupérer tout les Trucks de la base de données
                var trucksList = context.Trucks.ToList();

                // Comparer la plaque des camions de la base de données avec la plaque du camion actuel et si c'est différent ajouter le camion à la base de données
                if (!trucksList.Any(t => t.LicensePlate == truck.LicensePlate))
                {
                    context.Trucks.Add(truck);
                }
            }

            // Sauvegarder les changements dans la base de données
            await context.SaveChangesAsync();

            // Créer 10 commande factice pour éprouver le système
            // Récupérer les 4 premiers clients de la base de données
            var clients = context.Clients.Take(4).ToList();

            // Récupérer les 4 premiers camions de la base de données
            var trucks = context.Trucks.Take(5).ToList();

            // Récupérer les 4 premiers chauffeurs de la base de données
            var drivers = context.Drivers.Take(5).ToList();

            // Créer 10 livraisons factices
            for (int j = 0; j < 10 && context.Deliveries.Count() < 10; j++)
            {
                // Créer une adresse de départ factice
                var startAddress = new Address
                {
                    Street = "Rue de la rue",
                    Number = 1,
                    City = "Ville",
                    PostalCode = 1000,
                    Country = "Pays"
                };

                // Créer une adresse d'arrivée factice
                var endAddress = new Address
                {
                    Street = "Rue de la rue",
                    Number = 2,
                    City = "Ville",
                    PostalCode = 1000,
                    Country = "Pays"
                };

                // S'il y a moins de 2 adresses dans la base de données, ajouter l'adresse de départ à la base de données
                if (context.Addresses.Count() < 2)
                {
                    context.Addresses.Add(startAddress);
                    context.Addresses.Add(endAddress);

                    // Sauvegarder les changements dans la base de données
                    await context.SaveChangesAsync();
                }

                // Récupérer l'id des deux premières adresses de la base de données
                var startAddressId = context.Addresses.First().Id;
                var endAddressId = context.Addresses.Skip(1).First().Id;
                
                // Pour les 8 première livraisons
                if (j < 8)
                {
                    var delivery = new Delivery
                    {
                        LoadingLocationId = startAddressId,
                        LoadingLocation = startAddress,
                        UnloadingLocationId = endAddressId,
                        UnloadingLocation = endAddress,
                        DeliveryContent = "Contenu de la livraison" + j,
                        LoadingDateTime = DateTime.Now.AddHours(24 * (j % 4)),
                        UnloadingDateTime = DateTime.Now.AddHours(24 * (j % 4) + 2),
                        Client = clients[j % 4],
                        User = context.Users.First(u => u.Id == clients[j % 4].UserId),
                        DriverId = drivers[j % 5].UserId,
                        TruckId = trucks[j % 5].Id,
                        Status = DeliveryStatus.Assigned
                    };
                    // Ajouter la livraison à la base de données
                    context.Deliveries.Add(delivery);

                    // Sauvegarder les changements dans la base de données
                    await context.SaveChangesAsync();
                } else
                {
                    var delivery = new Delivery
                    {
                        LoadingLocationId = startAddressId,
                        LoadingLocation = startAddress,
                        UnloadingLocationId = endAddressId,
                        UnloadingLocation = endAddress,
                        DeliveryContent = "Contenu de la livraison" + j,
                        LoadingDateTime = DateTime.Now.AddHours(24 * (j % 4)),
                        UnloadingDateTime = DateTime.Now.AddHours(24 * (j % 4) + 2),
                        Client = clients[j % 4],
                        User = context.Users.First(u => u.Id == clients[j % 4].UserId),
                        Status = DeliveryStatus.Pending
                    };
                    // Ajouter la livraison à la base de données
                    context.Deliveries.Add(delivery);

                    // Sauvegarder les changements dans la base de données
                    await context.SaveChangesAsync();
                }

            }

        }
    }
}

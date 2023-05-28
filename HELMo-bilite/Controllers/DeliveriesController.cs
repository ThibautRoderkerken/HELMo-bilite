using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HELMo_bilite.Models;
using HELMo_bilite.ViewModels.Deliveries;
using Microsoft.AspNetCore.Identity;

namespace HELMo_bilite.Controllers
{
    public class DeliveriesController : Controller
    {
        private readonly HELMoBiliteDbContext _context;
        private readonly UserManager<User> _userManager;

        public DeliveriesController(HELMoBiliteDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> AdminGraphique()
        {
            // Récupérer une liste de livraisons
            var deliveries = await _context.Deliveries
                .Include(d => d.Driver)
                .Include(d => d.LoadingLocation)
                .Include(d => d.Truck)
                .Include(d => d.UnloadingLocation)
                .Include(d => d.User)
                .ToListAsync();

            // Parcourir toutes les livraisons
            foreach (var delivery in deliveries)
            {
                // Récupérer le client sur base UserId de la livraison
                var client = await _context.Clients
                    .FirstOrDefaultAsync(c => c.UserId == delivery.UserId);

                // Mettre à jour l'objet Client de la livraison
                delivery.Client = client;
            }

            // Parcourir toutes les livraisons et mettre a jour l'objet client
            foreach (var delivery in deliveries)
            {
                // Récupérer le Client sur base de UserId de la livraison
                var client = await _context.Clients
                    .FirstOrDefaultAsync(c => c.UserId == delivery.UserId);

                // Mettre à jour l'objet Client de la livraison
                delivery.Client = client;
            }

            // Grouper par chauffeur non null
            var deliveriesByDriver = deliveries
                .Where(d => d.Driver != null)
                .GroupBy(d => d.Driver.FirstName);


            // Créer une liste de livraisons par date(jour)
            var deliveriesByDate = deliveries.GroupBy(d => d.LoadingDateTime.Date);

            // Créer une liste de livraisons par client
            var deliveriesByClient = deliveries.GroupBy(d => d.Client);

            // Ajouter dans un ViewModel les 3 listes
            var AdminGraphique = new AdminGraphiqueViewModel
            {
                DeliveriesByDriver = deliveriesByDriver,
                DeliveriesByDate = deliveriesByDate,
                DeliveriesByClient = deliveriesByClient,
                Deliveries = deliveries
            };
            
            return View(AdminGraphique);
        }

        // GET: Index
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["LoadingDateSortParm"] = sortOrder == "loading_date" ? "loading_date_desc" : "loading_date";
            ViewData["UnloadingDateSortParm"] = sortOrder == "unloading_date" ? "unloading_date_desc" : "unloading_date";
            ViewData["StatusSortParm"] = sortOrder == "status" ? "status_desc" : "status";

            var currentUser = await _userManager.GetUserAsync(User);
            var currentUserId = currentUser.Id;
            var deliveriesForCurrentUser = _context.Deliveries
                .Include(d => d.Driver)
                .Include(d => d.LoadingLocation)
                .Include(d => d.Truck)
                .Include(d => d.UnloadingLocation)
                .Where(d => d.UserId == currentUserId);

            if (User.IsInRole("Admin"))
            {
                deliveriesForCurrentUser = _context.Deliveries
                .Include(d => d.Driver)
                .Include(d => d.LoadingLocation)
                .Include(d => d.Truck)
                .Include(d => d.UnloadingLocation);
            }

            // Trier en fonction du paramètre de tri
            switch (sortOrder)
            {
                case "status":
                    deliveriesForCurrentUser = deliveriesForCurrentUser.OrderBy(s => s.Status);
                    break;
                case "status_desc":
                    deliveriesForCurrentUser = deliveriesForCurrentUser.OrderByDescending(s => s.Status);
                    break;
                case "loading_date":
                    deliveriesForCurrentUser = deliveriesForCurrentUser.OrderBy(s => s.LoadingDateTime);
                    break;
                case "loading_date_desc":
                    deliveriesForCurrentUser = deliveriesForCurrentUser.OrderByDescending(s => s.LoadingDateTime);
                    break;
                case "unloading_date":
                    deliveriesForCurrentUser = deliveriesForCurrentUser.OrderBy(s => s.UnloadingDateTime);
                    break;
                case "unloading_date_desc":
                    deliveriesForCurrentUser = deliveriesForCurrentUser.OrderByDescending(s => s.UnloadingDateTime);
                    break;
                default:
                    deliveriesForCurrentUser = deliveriesForCurrentUser.OrderBy(s => s.Id); // Trier par ID par défaut
                    break;
            }

            return View(await deliveriesForCurrentUser.ToListAsync());
        }

        // GET: Admin
        public async Task<IActionResult> Admin(string sortOrder)
        {
            // Récupérer la liste des livraisons
            var deliveries = await _context.Deliveries
                .Include(d => d.Driver)
                .Include(d => d.LoadingLocation)
                .Include(d => d.Truck)
                .Include(d => d.UnloadingLocation)
                .Include(d => d.User)
                .ToListAsync();

            // Créer une liste de clients vide
            var clients = new List<Client>();

            // Parcourir toutes les livraisons
            foreach (var delivery in deliveries)
            {
                // Récupérer le client sur base UserId de la livraison
                var client = await _context.Clients
                    .FirstOrDefaultAsync(c => c.UserId == delivery.UserId);

                if (client != null)
                {
                    clients.Add(client);
                }
            }

            // Ne garder uniquement les clients uniques
            var uniqueClients = clients.Distinct();

            return View(uniqueClients);
        }


        // GET: Deliveries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Deliveries == null)
            {
                return NotFound();
            }

            var delivery = await _context.Deliveries
                .Include(d => d.Driver)
                .Include(d => d.LoadingLocation)
                .Include(d => d.Truck)
                .Include(d => d.UnloadingLocation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (delivery == null)
            {
                return NotFound();
            }

            return View(delivery);
        }

        // GET: Deliveries/Create
        public IActionResult Create()
        {
            ViewData["DriverId"] = new SelectList(_context.Drivers, "Id", "Id");
            ViewData["LoadingLocationId"] = new SelectList(_context.Addresses, "Id", "City");
            ViewData["TruckId"] = new SelectList(_context.Trucks, "Id", "LicensePlate");
            ViewData["UnloadingLocationId"] = new SelectList(_context.Addresses, "Id", "City");
            return View();
        }

        // POST: Deliveries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDeliveryViewModel deliveryViewModel)
        {
            if (ModelState.IsValid)
            {
                // Ajoutez les adresses à la base de données et obtenez leurs ID
                _context.Addresses.Add(deliveryViewModel.LoadingLocation);
                _context.Addresses.Add(deliveryViewModel.UnloadingLocation);
                await _context.SaveChangesAsync();

                var currentUser = await _userManager.GetUserAsync(User);
                var currentUserId = currentUser.Id;

                // Créez la livraison avec les ID des adresses et l'UserId
                Delivery delivery = new Delivery
                {
                    LoadingLocationId = deliveryViewModel.LoadingLocation.Id,
                    UnloadingLocationId = deliveryViewModel.UnloadingLocation.Id,
                    DeliveryContent = deliveryViewModel.DeliveryContent,
                    LoadingDateTime = deliveryViewModel.LoadingDateTime,
                    UnloadingDateTime = deliveryViewModel.UnloadingDateTime,
                    UserId = currentUserId,
                    Status = DeliveryStatus.Pending
                };

                // Ajoutez la livraison à la base de données
                _context.Deliveries.Add(delivery);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(deliveryViewModel);
        }

        // GET: Deliveries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var delivery = await _context.Deliveries
                .Include(d => d.LoadingLocation)
                .Include(d => d.UnloadingLocation)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (delivery == null)
            {
                return NotFound();
            }

            var editDeliveryViewModel = new EditDeliveryViewModel
            {
                Id = delivery.Id,
                LoadingLocation = delivery.LoadingLocation,
                UnloadingLocation = delivery.UnloadingLocation,
                DeliveryContent = delivery.DeliveryContent,
                LoadingDateTime = delivery.LoadingDateTime,
                UnloadingDateTime = delivery.UnloadingDateTime
            };

            ViewData["DriverId"] = new SelectList(_context.Drivers, "Id", "Id", delivery.DriverId);
            ViewData["TruckId"] = new SelectList(_context.Trucks, "Id", "LicensePlate", delivery.TruckId);

            return View(editDeliveryViewModel);
        }


        // POST: Deliveries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditDeliveryViewModel editDeliveryViewModel)
        {
            if (id != editDeliveryViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Mettre à jour les adresses dans la base de données
                    _context.Addresses.Update(editDeliveryViewModel.LoadingLocation);
                    _context.Addresses.Update(editDeliveryViewModel.UnloadingLocation);
                    await _context.SaveChangesAsync();

                    // Récupérer la livraison de la base de données
                    var deliveryDB = await _context.Deliveries.FindAsync(id);

                    // Vérifier si la livraison est en attente avant d'autoriser la mise à jour
                    if (deliveryDB.Status != DeliveryStatus.Pending)
                    {
                        // Rediriger vers une page d'erreur ou l'index si la livraison n'est pas en attente
                        return RedirectToAction(nameof(Index));
                    }

                    // Mettre à jour la livraison avec les ID des adresses
                    deliveryDB.LoadingLocationId = editDeliveryViewModel.LoadingLocation.Id;
                    deliveryDB.UnloadingLocationId = editDeliveryViewModel.UnloadingLocation.Id;
                    deliveryDB.DeliveryContent = editDeliveryViewModel.DeliveryContent;
                    deliveryDB.LoadingDateTime = editDeliveryViewModel.LoadingDateTime;
                    deliveryDB.UnloadingDateTime = editDeliveryViewModel.UnloadingDateTime;

                    // Pas besoin d'appeler Update, le contexte de données suit déjà cette entité
                    // _context.Deliveries.Update(delivery);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeliveryExists(editDeliveryViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["LoadingLocation"] = new SelectList(_context.Addresses, "Id", "City", editDeliveryViewModel.LoadingLocation);
            ViewData["UnloadingLocation"] = new SelectList(_context.Addresses, "Id", "City", editDeliveryViewModel.UnloadingLocation);

            return View(editDeliveryViewModel);
        }

        // GET: Deliveries/Tagguer/5
        public async Task<IActionResult> Tagguer(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Récupérer le client sur base de son id
            var customer = await _context.Clients.FindAsync(id);

            // Mettre le booléen Tagged à true
            customer.Tagged = true;

            // Mettre a jour le client dans la base de données
            _context.Clients.Update(customer);

            _context.SaveChanges();
            return RedirectToAction(nameof(Admin));
        }


        // GET: Deliveries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Deliveries == null)
            {
                return NotFound();
            }

            var delivery = await _context.Deliveries
                .Include(d => d.Driver)
                .Include(d => d.LoadingLocation)
                .Include(d => d.Truck)
                .Include(d => d.UnloadingLocation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (delivery == null)
            {
                return NotFound();
            }

            return View(delivery);
        }

        // GET: Deliveries/Pending
        public async Task<IActionResult> Pending()
        {
            // Récupérer les livraisons en attente
            var pendingDeliveries = await _context.Deliveries
                .Include(d => d.LoadingLocation)
                .Include(d => d.UnloadingLocation)
                .Where(d => d.Status == DeliveryStatus.Pending)
                .ToListAsync();

            // Créer une liste de DeliveryViewModel
            List<DeliveryViewModel> viewModel = new List<DeliveryViewModel>();

            // Parcourir chaque Delivery et créer un DeliveryViewModel correspondant
            foreach (var delivery in pendingDeliveries)
            {
                var user = await _context.Users.FindAsync(delivery.UserId);
                var customer = await _context.Clients.FindAsync(user.Id);

                viewModel.Add(new DeliveryViewModel
                {
                    Id = delivery.Id,
                    LoadingLocationId = delivery.LoadingLocationId,
                    LoadingLocation = delivery.LoadingLocation,
                    UnloadingLocationId = delivery.UnloadingLocationId,
                    UnloadingLocation = delivery.UnloadingLocation,
                    DeliveryContent = delivery.DeliveryContent,
                    LoadingDateTime = delivery.LoadingDateTime,
                    UnloadingDateTime = delivery.UnloadingDateTime,
                    DriverId = delivery.DriverId,
                    Driver = delivery.Driver,
                    TruckId = delivery.TruckId,
                    Truck = delivery.Truck,
                    UserId = delivery.UserId,
                    User = delivery.User,
                    Status = delivery.Status,
                    Comments = delivery.Comments,
                    FailedReason = delivery.FailedReason,
                    Tagged = customer?.Tagged ?? false
                });
            }

            // Retourner la vue avec les DeliveryViewModel
            return View(viewModel);
        }


        // GET: Deliveries/Assign/5
        public async Task<IActionResult> Assign(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var delivery = await _context.Deliveries
                .Include(d => d.LoadingLocation)
                .Include(d => d.UnloadingLocation)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (delivery == null)
            {
                return NotFound();
            }

            // Récupérer tous les chauffeurs et camions dans la base de données
            var allDrivers = await _context.Drivers.ToListAsync();
            var allTrucks = await _context.Trucks.ToListAsync();

            // Récupérer toutes les livraisons assignées
            var assignedDeliveries = await _context.Deliveries
                .Where(d => d.Status == DeliveryStatus.Assigned)
                .ToListAsync();

            // Définir les intervalles de temps pour la livraison actuelle
            var startInterval = delivery.LoadingDateTime.AddHours(-1);   // Prendre en compte l'heure de marge
            var endInterval = delivery.UnloadingDateTime.AddHours(1);    // Prendre en compte l'heure de marge

            // Filtrer les chauffeurs disponibles
            var availableDrivers = allDrivers.Where(driver =>
            {
                var driverDeliveries = assignedDeliveries.Where(d => d.DriverId == driver.UserId);
                return !driverDeliveries.Any(d => d.LoadingDateTime <= endInterval && d.UnloadingDateTime >= startInterval);
            }).ToList();

            // Filtrer les camions disponibles
            var availableTrucks = allTrucks.Where(truck =>
            {
                var truckDeliveries = assignedDeliveries.Where(d => d.TruckId == truck.Id);
                return !truckDeliveries.Any(d => d.LoadingDateTime <= endInterval && d.UnloadingDateTime >= startInterval);
            }).ToList();

            // Supprimer tout les chauffeur qui ont null ou b comme permis
            availableDrivers.RemoveAll(d => d.LicenseType == null || d.LicenseType == LicenseType.B);

            // Créer le ViewModel pour l'assignation
            var assignDeliveryViewModel = new AssignDeliveryViewModel
            {
                Delivery = delivery,
                UserId = availableDrivers.Any() ? availableDrivers.First().UserId : null,
                TruckId = availableTrucks.Any() ? availableTrucks.First().Id : 0,
                Drivers = new SelectList(availableDrivers.Select(d => new { d.UserId, Description = $"{d.FirstName} {d.LastName} - {d.LicenseType}" }), "UserId", "Description"),
                Trucks = new SelectList(availableTrucks.Select(t => new { t.Id, Description = $"{t.LicensePlate} - {t.Brand} - {t.Model} - {t.Type}" }), "Id", "Description")
            };

            return View(assignDeliveryViewModel);
        }


        // POST: Deliveries/Assign/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(int id, AssignDeliveryViewModel assignDeliveryViewModel)
        {
            if (id != assignDeliveryViewModel.Delivery.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Récupérer la livraison de la base de données
                var deliveryDB = await _context.Deliveries.FindAsync(id);

                // Vérifier si la livraison est toujours en attente
                if (deliveryDB.Status != DeliveryStatus.Pending)
                {
                    // Rediriger vers une page d'erreur ou l'index si la livraison n'est pas en attente
                    return RedirectToAction(nameof(Index));
                }

                // Assigner le chauffeur et le camion à la livraison
                deliveryDB.DriverId = assignDeliveryViewModel.UserId;
                deliveryDB.TruckId = assignDeliveryViewModel.TruckId;

                // Changer le statut de la livraison à "En cours"
                deliveryDB.Status = DeliveryStatus.Assigned;

                // Mettre à jour le statut du chauffeur et du camion à non disponible
                var driver = await _context.Drivers.FindAsync(assignDeliveryViewModel.UserId);
                var truck = await _context.Trucks.FindAsync(assignDeliveryViewModel.TruckId);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Pending));
            }

            return View(assignDeliveryViewModel);
        }

        private bool DeliveryExists(int id)
        {
            return _context.Deliveries.Any(e => e.Id == id);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HELMo_bilite.Models;
using HELMo_bilite.ViewModels.Admins;

namespace HELMo_bilite.Controllers
{
    public class AdminsController : Controller
    {
        private readonly HELMoBiliteDbContext _context;

        public AdminsController(HELMoBiliteDbContext context)
        {
            _context = context;
        }

        // GET: Admins
        public async Task<IActionResult> Index()
        {
              return _context.Admins != null ? 
                          View(await _context.Admins.ToListAsync()) :
                          Problem("Entity set 'HELMoBiliteDbContext.Admins'  is null.");
        }

        // GET: Admins/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Admins == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // GET: Admins/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                _context.Add(admin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(admin);
        }

        // GET: Admins/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Admins == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }
            return View(admin);
        }

        // POST: Admins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("UserId")] Admin admin)
        {
            if (id != admin.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(admin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminExists(admin.UserId))
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
            return View(admin);
        }

        // GET: Admins/ManageLicenses/5
        public async Task<IActionResult> ManageLicenses() 
        {
            // Récupérer la liste des Drivers
            var drivers = await _context.Drivers.ToListAsync();
            // Créer un ViewModel
            var viewModel = new List<EditLicenseViewModel>();
            // Pour chaque Driver
            foreach (var driver in drivers)
            {
                // Ajouter un EditLicenseViewModel à la liste
                viewModel.Add(new EditLicenseViewModel
                {
                    UserId = driver.UserId,
                    LicenseType = driver.LicenseType,
                    FirstName = driver.FirstName,
                    LastName = driver.LastName
                });
            }
            return View(viewModel);
        }

        // GET: Admins/EditLicense/5
        public async Task<IActionResult> EditLicense(string id)
        {
            if (id == null || _context.Drivers == null)
            {
                return NotFound();
            }

            var driver = await _context.Drivers.FindAsync(id);
            if (driver == null)
            {
                return NotFound();
            }

            // Créer un EditLicenseViewModel
            var viewModel = new EditLicenseViewModel
            {
                UserId = driver.UserId,
                LicenseType = driver.LicenseType,
                FirstName = driver.FirstName,
                LastName = driver.LastName
            };
            return View(viewModel);
        }

        // POST: Admins/EditLicense/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLicense(string id, EditLicenseViewModel driver)
        {
            if (id != driver.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Mettre a jour uniquement le type de permis
                try
                {
                    var driverToUpdate = await _context.Drivers.FindAsync(id);

                    // Si driver.LicenseType est null ou vaut B alors mettre toute les livraison du chauffeur dans l'état Pending et mettre le champs du camions a null
                    if (driver.LicenseType == null || driver.LicenseType == LicenseType.B)
                    {
                        // Récupérer toutes les livraisons du chauffeur dans l'état Assigned
                        var deliveries = await _context.Deliveries.Where(d => d.DriverId == driver.UserId && d.Status == DeliveryStatus.Assigned).ToListAsync();

                        foreach (var delivery in deliveries)
                        {
                            delivery.Status = DeliveryStatus.Pending;
                            delivery.TruckId = null;
                            delivery.DriverId = null;
                        }
                    } // Sinon vérifier dans toutes ses livraisons pour s'assurer qu'il possède toujours le permis minimum requis pour avoir l'autorisation de rouler avec le camion
                    else
                    {
                        // Récupérer toutes les livraisons du chauffeur dans l'état Assigned
                        var deliveries = await _context.Deliveries.Where(d => d.DriverId == driver.UserId && d.Status == DeliveryStatus.Assigned).ToListAsync();

                        foreach (var delivery in deliveries)
                        {
                            // Récupérer le camion de la livraison
                            var truck = await _context.Trucks.FindAsync(delivery.TruckId);

                            // Si le camion est null ou que le permis du camion est supérieur au permis du chauffeur alors mettre la livraison dans l'état Pending et mettre le champs du camions a null
                            if (truck == null || (driver.LicenseType == LicenseType.C && truck.Type == Truck.TruckType.CE))
                            {
                                delivery.Status = DeliveryStatus.Pending;
                                delivery.TruckId = null;
                                delivery.DriverId = null;
                            }
                        }
                    }


                    driverToUpdate.LicenseType = driver.LicenseType;
                    _context.Update(driverToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DriverExists(driver.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return RedirectToAction(nameof(ManageLicenses));
        }

        private bool DriverExists(string userId)
        {
            return (_context.Drivers?.Any(e => e.UserId == userId)).GetValueOrDefault();
        }


        // GET: Admins/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Admins == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // POST: Admins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Admins == null)
            {
                return Problem("Entity set 'HELMoBiliteDbContext.Admins'  is null.");
            }
            var admin = await _context.Admins.FindAsync(id);
            if (admin != null)
            {
                _context.Admins.Remove(admin);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdminExists(string id)
        {
          return (_context.Admins?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}

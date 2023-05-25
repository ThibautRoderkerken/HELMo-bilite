using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using HELMo_bilite.Models;
using Microsoft.EntityFrameworkCore;
using HELMo_bilite.ViewModels.Drivers;
using Microsoft.AspNetCore.Hosting;
using HELMo_bilite.ViewModels.Registers;
using Microsoft.AspNetCore.Authorization;

namespace HELMo_bilite.Controllers
{
    public class DriversController : Controller
    {
        private readonly HELMoBiliteDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public DriversController(HELMoBiliteDbContext context, UserManager<User> userManager, SignInManager<User> signInManager, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Drivers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Drivers.ToListAsync());
        }

        // GET: Drivers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                var curentUser = await _userManager.GetUserAsync(User);
                if (curentUser != null)
                {
                    id = curentUser.Id;
                }
                else
                {
                    return NotFound();
                }
            }

            var driver = await _context.Drivers.FirstOrDefaultAsync(m => m.UserId == id);

            // You can add a role check here to make sure the user is either a Driver or an Admin.
            // Get the user and their roles:
            var currentUser = await _userManager.GetUserAsync(User);
            var userRoles = await _userManager.GetRolesAsync(currentUser);

            if (driver == null)
            {
                return NotFound();
            }
            else if (!(userRoles.Contains("Admin") || currentUser.Id == id))
            {
                // if the user is not an Admin and is not viewing their own details, forbid access.
                return Forbid();
            }

            return View(driver);
        }


        // GET: Drivers/Register
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Drivers/Register
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDriverViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var driver = new Driver
                    {
                        UserId = user.Id,
                        Matricule = model.Matricule,
                        LastName = model.LastName,
                        FirstName = model.FirstName,
                        LicenseType = model.LicenseType
                    };

                    // Add the driver to the Drivers table
                    _context.Add(driver);

                    // Assign the driver role to the user
                    await _userManager.AddToRoleAsync(user, "Driver");

                    await _context.SaveChangesAsync();
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        // GET: Drivers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Forbid();
            }

            if (currentUser.Id != id)
            {
                return Forbid();
            }

            var driver = await _context.Drivers.FindAsync(id);
            if (driver == null)
            {
                return NotFound();
            }

            var model = new EditDriverViewModel
            {
                Matricule = driver.Matricule,
                LastName = driver.LastName,
                FirstName = driver.FirstName,
                LicenseType = driver.LicenseType ?? LicenseType.B,
                DateOfBirth = driver.DateOfBirth
            };

            return View(model);
        }

        // POST: Drivers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditDriverViewModel model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Forbid();
            }

            if (currentUser.Id != id)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                Driver driver = null;

                try
                {
                    driver = await _context.Drivers.FindAsync(id);
                    if (driver == null)
                    {
                        return NotFound();
                    }

                    driver.Matricule = model.Matricule;
                    driver.LastName = model.LastName;
                    driver.FirstName = model.FirstName;
                    driver.LicenseType = model.LicenseType;
                    driver.DateOfBirth = model.DateOfBirth;

                    if (model.NewPhoto != null)
                    {
                        var dirPath = Path.Combine(_hostingEnvironment.WebRootPath, "images/drivers");
                        var fileName = Guid.NewGuid().ToString() + "_" + model.NewPhoto.FileName;
                        var filePath = Path.Combine(dirPath, fileName);

                        if (!Directory.Exists(dirPath))
                        {
                            Directory.CreateDirectory(dirPath);
                        }

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.NewPhoto.CopyToAsync(fileStream);
                        }

                        // update the photo path of the driver
                        driver.Photo = "/images/drivers/" + fileName;
                    }

                    _context.Update(driver);
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
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }


        // GET: Drivers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var driver = await _context.Drivers
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (driver == null)
            {
                return NotFound();
            }

            return View(driver);
        }

        // POST: Drivers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var driver = await _context.Drivers.FindAsync(id);
            _context.Drivers.Remove(driver);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DriverExists(string id)
        {
            return _context.Drivers.Any(e => e.UserId == id);
        }

        public IActionResult Calendar()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Deliveries()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var driver = await _context.Drivers.FirstOrDefaultAsync(d => d.UserId == user.Id);
            if (driver == null)
            {
                return NotFound();
            }

            var deliveries = await _context.Deliveries
                .Include(d => d.LoadingLocation)
                .Include(d => d.UnloadingLocation)
                .Where(d => d.DriverId == driver.UserId && d.Status == DeliveryStatus.Assigned)
                .ToListAsync();

            return Json(deliveries);
        }



        [HttpPost]
        public async Task<IActionResult> ManageDelivery(int deliveryId)
        {
            // Récupérez la livraison avec l'ID spécifié.
            var delivery = await _context.Deliveries.FindAsync(deliveryId);

            // Si aucune livraison n'est trouvée avec l'ID spécifié, renvoyez NotFound.
            if (delivery == null)
            {
                return NotFound();
            }

            // Sinon, passez la livraison à la vue.
            return View(delivery);
        }


        private bool DeliveryExists(int id)
        {
            return _context.Deliveries.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDeliveryStatus(int deliveryId, string deliveryStatus, string failedReason, string comments)
        {
            // Récupérer la livraison de la base de données
            var delivery = await _context.Deliveries.FindAsync(deliveryId);

            if (delivery == null)
            {
                return NotFound();
            }

            // Vérifier le statut de la livraison
            if (deliveryStatus == "Delivered")
            {
                delivery.Status = DeliveryStatus.Completed;
            }
            else if (deliveryStatus == "Failed")
            {
                delivery.Status = DeliveryStatus.Failed;
            }

            // Ajouter le commentaire
            delivery.Comments = comments;

            // Si la livraison a échoué, ajouter la raison de l'échec
            if (deliveryStatus == "Failed")
            {
                delivery.FailedReason = failedReason;
            }

            // Sauvegarder les modifications dans la base de données
            await _context.SaveChangesAsync();

            // Rediriger vers le calendrier
            return RedirectToAction("Calendar");
        }




        private string UploadedFile(EditDriverViewModel model)
        {
            string uniqueFileName = null;

            if (model.NewPhoto != null)
            {
                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images/drivers");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.NewPhoto.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.NewPhoto.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
    }
}

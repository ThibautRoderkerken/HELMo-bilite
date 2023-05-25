using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using HELMo_bilite.Models;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using HELMo_bilite.ViewModels.Trucks;

namespace HELMo_bilite.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TrucksController : Controller
    {
        private readonly HELMoBiliteDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public TrucksController(HELMoBiliteDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Trucks
        public async Task<IActionResult> Index()
        {
            return View(await _context.Trucks.ToListAsync());
        }

        // GET: Trucks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var truck = await _context.Trucks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (truck == null)
            {
                return NotFound();
            }

            return View(truck);
        }

        // GET: Trucks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Trucks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTruckViewModel truck)
        {
            if (ModelState.IsValid)
            {
                // Save image to wwwroot/images/trucks
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(truck.ImageFile.FileName);
                string extension = Path.GetExtension(truck.ImageFile.FileName);
                truck.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;

                // vérifier que les dossiers existent
                string imagePath = Path.Combine(wwwRootPath, "images", "trucks");
                if (!Directory.Exists(imagePath))
                {
                    Directory.CreateDirectory(imagePath);
                }

                // Créer le chemin complet de l'image
                string path = Path.Combine(imagePath, fileName);

                // Sauvegarder l'image
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await truck.ImageFile.CopyToAsync(fileStream);
                }

                // Convertir et sauvegarder le camion
                Truck newTruck = new Truck
                {
                    Brand = truck.Brand,
                    Model = truck.Model,
                    LicensePlate = truck.LicensePlate,
                    Type = truck.Type,
                    Payload = truck.Payload,
                    ImageName = "trucks/" + truck.ImageName // Ne pas oublier d'ajouter "trucks/" dans le nom de l'image pour correspondre au nouveau chemin
                };
                _context.Add(newTruck);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(truck);
        }


        // GET: Trucks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var truck = await _context.Trucks.FindAsync(id);
            if (truck == null)
            {
                return NotFound();
            }

            var editTruckViewModel = new EditTruckViewModel
            {
                Id = truck.Id,
                Brand = truck.Brand,
                Model = truck.Model,
                LicensePlate = truck.LicensePlate,
                Type = truck.Type,
                Payload = truck.Payload,
                ExistingImageName = truck.ImageName
            };

            return View(editTruckViewModel);
        }


        // POST: Trucks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditTruckViewModel truck)
        {
            if (id != truck.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string existingImageName = truck.ExistingImageName;

                    // If a new image file has been uploaded
                    if (truck.ImageFile != null)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(truck.ImageFile.FileName);
                        string extension = Path.GetExtension(truck.ImageFile.FileName);
                        string imageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;

                        // Delete existing image
                        var existingImagePath = Path.Combine(wwwRootPath, "images", existingImageName);
                        if (System.IO.File.Exists(existingImagePath))
                            System.IO.File.Delete(existingImagePath);

                        // Save new image
                        string imagePath = Path.Combine(wwwRootPath, "images", "trucks");
                        if (!Directory.Exists(imagePath))
                        {
                            Directory.CreateDirectory(imagePath);
                        }

                        string newPath = Path.Combine(imagePath, fileName);
                        using (var fileStream = new FileStream(newPath, FileMode.Create))
                        {
                            await truck.ImageFile.CopyToAsync(fileStream);
                        }

                        // Only update image name if a new file has been uploaded
                        existingImageName = "trucks/" + imageName;
                    }

                    // Update truck
                    var updatedTruck = new Truck
                    {
                        Id = truck.Id,
                        Brand = truck.Brand,
                        Model = truck.Model,
                        LicensePlate = truck.LicensePlate,
                        Type = truck.Type,
                        Payload = truck.Payload,
                        ImageName = existingImageName
                    };

                    _context.Update(updatedTruck);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TruckExists(truck.Id))
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
            return View(truck);
        }

        // GET: Trucks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var truck = await _context.Trucks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (truck == null)
            {
                return NotFound();
            }

            return View(truck);
        }
        // POST: Trucks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var truck = await _context.Trucks.FindAsync(id);

            // Delete image from wwwroot/images/trucks
            var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "images", truck.ImageName);
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);

            _context.Trucks.Remove(truck);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool TruckExists(int id)
        {
            return _context.Trucks.Any(e => e.Id == id);
        }
    }
}

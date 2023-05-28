using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using HELMo_bilite.Models;
using Microsoft.EntityFrameworkCore;
using HELMo_bilite.ViewModels.Registers;
using Microsoft.AspNetCore.Authorization;
using HELMo_bilite.ViewModels.Dispatchers;

namespace HELMo_bilite.Controllers
{
    public class DispatchersController : Controller
    {
        private readonly HELMoBiliteDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public DispatchersController(HELMoBiliteDbContext context, UserManager<User> userManager, SignInManager<User> signInManager, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Dispatchers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Dispatchers.ToListAsync());
        }

        // GET: Dispatchers/Details/5
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

            var dispatcher = await _context.Dispatchers.FirstOrDefaultAsync(m => m.UserId == id);

            // You can add a role check here to make sure the user is either a Dispatcher or an Admin.
            // Get the user and their roles:
            var currentUser = await _userManager.GetUserAsync(User);
            var userRoles = await _userManager.GetRolesAsync(currentUser);

            if (dispatcher == null)
            {
                return NotFound();
            }
            else if (!(userRoles.Contains("Admin") || currentUser.Id == id))
            {
                // if the user is not an Admin and is not viewing their own details, forbid access.
                return Forbid();
            }

            return View(dispatcher);
        }

        // GET: Dispatchers/Register
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Dispatchers/Register
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDispatcherViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var dispatcher = new Dispatcher
                    {
                        UserId = user.Id,
                        Matricule = model.Matricule,
                        LastName = model.LastName,
                        FirstName = model.FirstName,
                        EducationLevel = model.EducationLevel
                    };

                    // Add the dispatcher to the Dispatchers table
                    _context.Add(dispatcher);

                    // Assign the dispatcher role to the user
                    await _userManager.AddToRoleAsync(user, "Dispatcher");

                    await _context.SaveChangesAsync();
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction(nameof(Details));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        // GET: Dispatchers/Edit/5
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

            var dispatcher = await _context.Dispatchers.FindAsync(id);
            if (dispatcher == null)
            {
                return NotFound();
            }

            var model = new EditDispatcherViewModel
            {
                Matricule = dispatcher.Matricule,
                LastName = dispatcher.LastName,
                FirstName = dispatcher.FirstName,
                EducationLevel = dispatcher.EducationLevel.ToString(),
                DateOfBirth = dispatcher.DateOfBirth
            };

            return View(model);
        }

        // POST: Dispatchers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditDispatcherViewModel model)
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
                Dispatcher dispatcher = null;

                try
                {
                    dispatcher = await _context.Dispatchers.FindAsync(id);
                    if (dispatcher == null)
                    {
                        return NotFound();
                    }

                    dispatcher.Matricule = model.Matricule;
                    dispatcher.LastName = model.LastName;
                    dispatcher.FirstName = model.FirstName;
                    dispatcher.EducationLevel = Enum.Parse<EducationLevel>(model.EducationLevel);
                    dispatcher.DateOfBirth = model.DateOfBirth;

                    if (model.NewPhoto != null)
                    {
                        var fileName = Path.GetFileName(model.NewPhoto.FileName);
                        var dirPath = Path.Combine(_hostingEnvironment.WebRootPath, "images", "dispatchers");
                        var filePath = Path.Combine(dirPath, fileName);

                        if (!Directory.Exists(dirPath))
                        {
                            Directory.CreateDirectory(dirPath);
                        }

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.NewPhoto.CopyToAsync(fileStream);
                        }

                        // If the application is running in production, prefix the image URL with '/Q210040'
                        var imageUrlPrefix = _hostingEnvironment.IsProduction() ? "/Q210040" : "";

                        dispatcher.Photo = imageUrlPrefix + "/images/dispatchers/" + fileName;
                    }

                    _context.Update(dispatcher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DispatcherExists(dispatcher.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details));
            }
            return View(model);
        }


        // GET: Dispatchers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dispatcher = await _context.Dispatchers
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (dispatcher == null)
            {
                return NotFound();
            }

            return View(dispatcher);
        }

        // POST: Dispatchers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var dispatcher = await _context.Dispatchers.FindAsync(id);
            _context.Dispatchers.Remove(dispatcher);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DispatcherExists(string id)
        {
            return _context.Dispatchers.Any(e => e.UserId == id);
        }

        private string UploadedFile(EditDispatcherViewModel model)
        {
            string uniqueFileName = null;

            if (model.NewPhoto != null)
            {
                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images/dispatchers");
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

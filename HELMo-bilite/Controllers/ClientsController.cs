using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using HELMo_bilite.Models;
using Microsoft.AspNetCore.Authorization;
using HELMo_bilite.ViewModels.Registers;
using HELMo_bilite.ViewModels.Clients;

namespace HELMo_bilite.Controllers
{
    [Authorize(Roles = "Client,Admin")]
    public class ClientsController : Controller
        {
            private readonly HELMoBiliteDbContext _context;
            private readonly UserManager<User> _userManager;
            private readonly SignInManager<User> _signInManager;
            private readonly IWebHostEnvironment _hostingEnvironment;

            public ClientsController(HELMoBiliteDbContext context, UserManager<User> userManager, SignInManager<User> signInManager, IWebHostEnvironment webHostEnvironment)
            {
                _context = context;
                _userManager = userManager;
                _signInManager = signInManager;
                _hostingEnvironment = webHostEnvironment;
            }

            // GET: Clients
            public async Task<IActionResult> Index()
            {
                return View(await _context.Clients.ToListAsync());
            }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    id = currentUser.Id;
                }
                else
                {
                    return NotFound();
                }
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }


        // GET: Clients/Register
        [AllowAnonymous]
        public IActionResult Register()
            {
                return View();
            }

        // POST: Clients/Register
        [AllowAnonymous]
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Register(RegisterClientViewModel model)
            {
                if (ModelState.IsValid)
                {
                    var user = new User { UserName = model.Email, Email = model.Email };
                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        // Create a new Client instance
                        var client = new Client
                        {
                            UserId = user.Id,
                            CompanyName = model.CompanyName,
                            Street = model.Street,
                            Number = model.Number,
                            City = model.City,
                            PostalCode = model.PostalCode,
                            Country = model.Country
                        };

                        // Add the client to the Clients table
                        _context.Add(client);

                        // Assign the client role to the user
                        await _userManager.AddToRoleAsync(user, "Client");

                        await _context.SaveChangesAsync();

                        // Sign in the user here
                        await _signInManager.SignInAsync(user, isPersistent: false);

                        return RedirectToAction(nameof(Index));
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

                // If we got this far, something failed, redisplay form
                return View(model);
            }


        [Authorize]
        public async Task<IActionResult> Edit(string id)
        {
            // Check if the user is authenticated
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                // Not authenticated
                return Forbid();
            }

            // Check if the user is the one being edited
            if (currentUser.Id != id)
            {
                // Not authorized
                return Forbid();
            }

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            var model = new EditClientViewModel
            {
                CompanyName = client.CompanyName,
                Street = client.Street,
                Number = client.Number,
                City = client.City,
                PostalCode = client.PostalCode,
                Country = client.Country,
                CurrentCompanyLogoPath = client.CompanyLogoPath
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(string id, EditClientViewModel model)
        {
            // Check if the user is authenticated
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                // Not authenticated
                return Forbid();
            }

            // Check if the user is the one being edited
            if (currentUser.Id != id)
            {
                // Not authorized
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                Client client = null;

                try
                {
                    client = await _context.Clients.FindAsync(id);
                    if (client == null)
                    {
                        return NotFound();
                    }

                    // Copy the fields that we want to update
                    client.CompanyName = model.CompanyName;
                    client.Street = model.Street;
                    client.Number = model.Number;
                    client.City = model.City;
                    client.PostalCode = model.PostalCode;
                    client.Country = model.Country;

                    // Handle the uploaded file
                    if (model.NewCompanyLogo != null)
                    {
                        var fileName = Path.GetFileName(model.NewCompanyLogo.FileName);
                        var dirPath = Path.Combine(_hostingEnvironment.WebRootPath, "images", "clients");
                        var filePath = Path.Combine(dirPath, fileName);

                        // Check if the directory exists and create it if not
                        if (!Directory.Exists(dirPath))
                        {
                            Directory.CreateDirectory(dirPath);
                        }

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.NewCompanyLogo.CopyToAsync(stream);
                        }

                        client.CompanyLogoPath = "/images/clients/" + fileName;
                    }


                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client?.UserId))
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

        [Authorize(Roles = "Admin")]
        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(string id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var client = await _context.Clients
                    .FirstOrDefaultAsync(m => m.UserId == id);
                if (client == null)
                {
                    return NotFound();
                }

                // Vérifier que c'est bien le client logguer qui essaie de supprimer son comtpe
                var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                    // Not authenticated
                    return Forbid();
                }

                return View(client);
            }

            // POST: Clients/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteConfirmed(string id)
            {
                var client = await _context.Clients.FindAsync(id);
                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            private bool ClientExists(string id)
            {
                return _context.Clients.Any(e => e.UserId == id);
            }
        }
    }

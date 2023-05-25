using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HELMo_bilite.Models;
using Microsoft.AspNetCore.Authorization;


namespace HELMo_bilite.Controllers
{
    [Authorize]

    public class DriverLicensesController : Controller
    {
        private readonly HELMoBiliteDbContext _context;

        public DriverLicensesController(HELMoBiliteDbContext context)
        {
            _context = context;
        }

        // GET: DriverLicenses
        public async Task<IActionResult> Index()
        {
            var hELMoBiliteDbContext = _context.DriverLicenses.Include(d => d.Driver);
            return View(await hELMoBiliteDbContext.ToListAsync());
        }

        // GET: DriverLicenses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.DriverLicenses == null)
            {
                return NotFound();
            }

            var driverLicense = await _context.DriverLicenses
                .Include(d => d.Driver)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (driverLicense == null)
            {
                return NotFound();
            }

            return View(driverLicense);
        }

        // GET: DriverLicenses/Create
        public IActionResult Create()
        {
            ViewData["DriverId"] = new SelectList(_context.Drivers, "Id", "Id");
            return View();
        }

        // POST: DriverLicenses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LicenseType,DriverId")] DriverLicense driverLicense)
        {
            if (ModelState.IsValid)
            {
                _context.Add(driverLicense);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DriverId"] = new SelectList(_context.Drivers, "Id", "Id", driverLicense.DriverId);
            return View(driverLicense);
        }

        // GET: DriverLicenses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.DriverLicenses == null)
            {
                return NotFound();
            }

            var driverLicense = await _context.DriverLicenses.FindAsync(id);
            if (driverLicense == null)
            {
                return NotFound();
            }
            ViewData["DriverId"] = new SelectList(_context.Drivers, "Id", "Id", driverLicense.DriverId);
            return View(driverLicense);
        }

        // POST: DriverLicenses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LicenseType,DriverId")] DriverLicense driverLicense)
        {
            if (id != driverLicense.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(driverLicense);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DriverLicenseExists(driverLicense.Id))
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
            ViewData["DriverId"] = new SelectList(_context.Drivers, "Id", "Id", driverLicense.DriverId);
            return View(driverLicense);
        }

        // GET: DriverLicenses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.DriverLicenses == null)
            {
                return NotFound();
            }

            var driverLicense = await _context.DriverLicenses
                .Include(d => d.Driver)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (driverLicense == null)
            {
                return NotFound();
            }

            return View(driverLicense);
        }

        // POST: DriverLicenses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.DriverLicenses == null)
            {
                return Problem("Entity set 'HELMoBiliteDbContext.DriverLicenses'  is null.");
            }
            var driverLicense = await _context.DriverLicenses.FindAsync(id);
            if (driverLicense != null)
            {
                _context.DriverLicenses.Remove(driverLicense);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DriverLicenseExists(int id)
        {
          return (_context.DriverLicenses?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System;
using HELMo_bilite.Models;
using Microsoft.Extensions.Logging;

namespace HELMo_bilite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _hostEnvironment;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;

            // Get truck images
            string truckImagePath = Path.Combine(wwwRootPath, "images", "trucks");
            var truckImageFiles = Directory.GetFiles(truckImagePath).Select(Path.GetFileName).ToList();

            if (!truckImageFiles.Any())
            {
                ViewBag.TruckMessage = "Il n'y a pas d'images des camions dans le système. Veuillez en ajouter.";
            }
            else
            {
                var selectedTruckImages = truckImageFiles.OrderBy(f => f).Take(4);
                ViewBag.TruckImages = selectedTruckImages;
            }

            // Get client images
            string clientImagePath = Path.Combine(wwwRootPath, "images", "clients");
            var clientImageFiles = Directory.GetFiles(clientImagePath).Select(Path.GetFileName).ToList();

            if (!clientImageFiles.Any())
            {
                ViewBag.ClientMessage = "Il n'y a pas d'images des clients dans le système. Veuillez en ajouter.";
            }
            else
            {
                var selectedClientImages = clientImageFiles.OrderBy(f => f);
                ViewBag.ClientImages = selectedClientImages;
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

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
            string imagePath = Path.Combine(wwwRootPath, "images", "trucks");

            // Lire tous les fichiers dans le dossier
            var imageFiles = Directory.GetFiles(imagePath).Select(Path.GetFileName).ToList();

            // Si aucun fichier n'est présent, mettre un message dans ViewBag.Message
            if (!imageFiles.Any())
            {
                ViewBag.Message = "Il n'y a pas d'images dans le système. Veuillez en ajouter.";
            }
            else
            {
                // Récupérer les 4 premire image par ordre alphabétique
                var selectedImages = imageFiles.OrderBy(f => f).Take(4);

                // Passez les images à la vue
                ViewBag.Images = selectedImages;
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

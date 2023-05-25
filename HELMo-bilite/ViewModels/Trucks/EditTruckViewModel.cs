using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using static HELMo_bilite.Models.Truck;

namespace HELMo_bilite.ViewModels.Trucks
{
    public class EditTruckViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Marque")]
        public TruckBrand Brand { get; set; }

        [Required]
        [Display(Name = "Modèle")]
        public string Model { get; set; }

        [Required]
        [RegularExpression(@"^\d{1}-[A-Z]{3}-\d{3}$", ErrorMessage = "Invalid License Plate")]
        [Display(Name = "Plaque d'immatriculation")]
        public string LicensePlate { get; set; }

        [Required]
        [Display(Name = "Type")]
        public TruckType Type { get; set; }

        [Required]
        [Range(1, 100)]
        [Display(Name = "Charge utile")]
        public int Payload { get; set; }

        [NotMapped]
        [Display(Name = "Image")]
        public IFormFile? ImageFile { get; set; }

        [Display(Name = "Image")]
        public string? ExistingImageName { get; set; }
    }
}

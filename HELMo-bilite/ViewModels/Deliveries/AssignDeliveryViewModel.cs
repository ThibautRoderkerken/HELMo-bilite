using System.ComponentModel.DataAnnotations;
using HELMo_bilite.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HELMo_bilite.ViewModels.Deliveries
{
    public class AssignDeliveryViewModel
    {
        [Display(Name = "Delivery")]
        public Delivery Delivery { get; set; }

        [Display(Name = "Available drivers")]
        public SelectList? Drivers { get; set; }

        [Display(Name = "Available drivers")]
        [Required(ErrorMessage = "Veuillez sélectionner un chauffeur.")]
        public string? UserId { get; set; }

        [Display(Name = "Available trucks")]
        public SelectList? Trucks { get; set; }

        [Display(Name = "Available trucks")]
        [Required(ErrorMessage = "Veuillez sélectionner un camion.")]
        public int? TruckId { get; set; }
    }
}


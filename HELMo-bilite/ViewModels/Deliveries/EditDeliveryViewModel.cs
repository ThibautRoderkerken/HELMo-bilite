using HELMo_bilite.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace HELMo_bilite.ViewModels.Deliveries
{
    public class EditDeliveryViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Lieu de chargement")]
        public Address LoadingLocation { get; set; }

        [Required]
        [Display(Name = "Lieux de déchargement")]
        public Address UnloadingLocation { get; set; }

        [Required]
        [Display(Name = "Contenu de la livraison")]
        public string DeliveryContent { get; set; }

        [Required]
        [Display(Name = "Date et heure de chargement")]
        public DateTime LoadingDateTime { get; set; }

        [Required]
        [Display(Name = "Date et heure de déchargement")]
        [DateGreaterThan("LoadingDateTime")]
        public DateTime UnloadingDateTime { get; set; }
    }
}

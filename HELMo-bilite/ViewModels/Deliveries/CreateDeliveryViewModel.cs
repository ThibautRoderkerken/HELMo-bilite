using HELMo_bilite.Models;
using System.ComponentModel.DataAnnotations;

namespace HELMo_bilite.ViewModels.Deliveries
{
    public class CreateDeliveryViewModel
    {
        [Required]
        [Display(Name = "Loading location")]
        public Address LoadingLocation { get; set; }

        [Required]
        [Display(Name = "Unloading location")]
        public Address UnloadingLocation { get; set; }

        [Required]
        [Display(Name = "Delivery content")]
        public string DeliveryContent { get; set; }

        [Required]
        [Display(Name = "Loading date and time")]
        public DateTime LoadingDateTime { get; set; }

        [Required]
        [Display(Name = "Unloading date and time")]
        [DateGreaterThan("LoadingDateTime")]
        public DateTime UnloadingDateTime { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace HELMo_bilite.ViewModels.Clients
{
    public class EditClientViewModel
    {
        public string? UserId { get; set;}

        [Required]
        [Display(Name = "Company name")]
        public string CompanyName { get; set; }

        [Required]
        [Display(Name = "Street")]
        public string Street { get; set; }

        [Required]
        [Display(Name = "Number")]
        public int Number { get; set; }

        [Required]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required]
        [Display(Name = "Postal code")]
        public int PostalCode { get; set; }

        [Required]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [Display(Name = "Current company logo")]
        public string? CurrentCompanyLogoPath { get; set; }

        [Display(Name = "New company logo")]
        public IFormFile? NewCompanyLogo { get; set; }
    }
}

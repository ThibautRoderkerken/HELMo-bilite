using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using HELMo_bilite.Models;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace HELMo_bilite.Models
{
    public class Client
    {
        [Key]
        [ForeignKey("UserId")]
        public string UserId { get; set; }

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

        
        [Display(Name = "Company logo")]
        public string? CompanyLogoPath { get; set; }

        [NotMapped]
        [Display(Name = "Company logo")]
        public IFormFile? CompanyLogo { get; set; }
        public bool? Tagged { get; internal set; }
    }
}

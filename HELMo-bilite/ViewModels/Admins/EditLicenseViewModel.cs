using System.ComponentModel.DataAnnotations;
using HELMo_bilite.Models;
using Microsoft.AspNetCore.Http;

namespace HELMo_bilite.ViewModels.Admins
{
    public class EditLicenseViewModel
    {
        public string? UserId { get; set;}

        [Display(Name = "License type")]
        public LicenseType? LicenseType { get; set; }

        [Required]
        [Display(Name = "First name")]
        public string? FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string? LastName { get; set; }
    }
}

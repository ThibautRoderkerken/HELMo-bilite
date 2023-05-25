using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using HELMo_bilite.Models;

namespace HELMo_bilite.ViewModels.Drivers
{
    public class EditDriverViewModel
    {
        public string? UserId { get; set; }

        [Required]
        [Display(Name = "Matricule")]
        public string Matricule { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "License type")]
        public LicenseType LicenseType { get; set; }

        [Display(Name = "Date of birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Current photo")]
        public string? CurrentPhotoPath { get; set; }

        [Display(Name = "New photo")]
        public IFormFile? NewPhoto { get; set; }
    }
}

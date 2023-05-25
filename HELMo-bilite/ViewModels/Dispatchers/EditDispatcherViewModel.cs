using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace HELMo_bilite.ViewModels.Dispatchers
{
    public class EditDispatcherViewModel
    {
        public string? UserId { get; set; }

        [Required]
        [Display(Name = "Matricule")]
        public string Matricule { get; set; }

        [Required]
        [Display(Name = "Nom")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Prénom")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Niveau d'étude maximum effectué")]
        public string EducationLevel { get; set; }

        [Display(Name = "Date de naissance")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Photo actuelle")]
        public string? CurrentPhotoPath { get; set; }

        [Display(Name = "Nouvelle photo")]
        public IFormFile? NewPhoto { get; set; }

        public string[] EducationLevels { get; } = { "CESS", "Bachelier", "Licencier" };

    }
}

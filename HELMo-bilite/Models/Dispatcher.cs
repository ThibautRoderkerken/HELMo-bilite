using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HELMo_bilite.Models
{
    public class Dispatcher : HELMoMember
    {
        [Key]
        [ForeignKey("UserId")]
        public string UserId { get; set; }

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
        [Display(Name = "Max education level")]
        public EducationLevel EducationLevel { get; set; }
    }

    public enum EducationLevel
    {
        CESS,
        Bachelier,
        Licencier
    }
}

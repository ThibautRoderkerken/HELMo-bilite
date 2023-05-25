using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Permissions;
using System.Security.Policy;

namespace HELMo_bilite.Models
{
    public class Driver : HELMoMember
    {

        [Key]
        [ForeignKey("UserId")]
        public string UserId { get; set; }
        [Required]
        public string Matricule { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "License Type")]
        public LicenseType? LicenseType { get; set; }

    }

    public enum LicenseType
    {
        B = 1,
        C = 2,
        CE = 4
    }
}

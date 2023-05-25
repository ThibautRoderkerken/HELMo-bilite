using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HELMo_bilite.Models
{
    public class DriverLicense
    {
        [Key]
        public int Id { get; set; }

        public LicenseType LicenseType { get; set; }

        [Required]
        [ForeignKey("DriverId")]
        public Driver Driver { get; set; }
        public string DriverId { get; set; }
    }

}

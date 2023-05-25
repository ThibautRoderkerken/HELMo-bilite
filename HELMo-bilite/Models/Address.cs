using System.ComponentModel.DataAnnotations;

namespace HELMo_bilite.Models
{
    public class Address
    {
        [Key]
        public int Id { get; set; }

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
        [Display(Name = "Zip code")]
        public int PostalCode { get; set; }

        [Required]
        [Display(Name = "Country")]
        public string Country { get; set; }
    }
}

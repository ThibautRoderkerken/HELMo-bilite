using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HELMo_bilite.Models
{
    public class Truck
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Brand")]
        public TruckBrand Brand { get; set; }

        [Required]
        [Display(Name = "Model")]
        public string Model { get; set; }

        [Required]
        [RegularExpression(@"^\d{1}-[A-Z]{3}-\d{3}$", ErrorMessage = "Invalid License Plate")]
        [Display(Name = "License plate")]
        public string LicensePlate { get; set; }

        [Required]
        [Display(Name = "Type")]
        public TruckType Type { get; set; }

        [Required]
        [Range(1, 100)]
        [Display(Name = "Payload")]
        public int Payload { get; set; }

        [NotMapped]
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }

        [Display(Name = "Image")]
        public string ImageName { get; set; }

        public enum TruckBrand
        {
            Mercedes,
            Volvo,
            MAN,
            Iveco,
            Scania,
            DAF,
            Renault,
            Kenworth,
            Peterbilt,
            Mack
        }

        public enum TruckType
        {
            C,
            CE
        }
    }
}

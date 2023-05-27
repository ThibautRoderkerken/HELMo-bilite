using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HELMo_bilite.Models
{
    public class Delivery
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Loading location")]
        [ForeignKey("LoadingLocation")]
        public int LoadingLocationId { get; set; }
        [Display(Name = "Loading location")]
        public Address LoadingLocation { get; set; }

        [Required]
        [Display(Name = "Unloading location")]
        [ForeignKey("UnloadingLocation")]
        public int UnloadingLocationId { get; set; }
        [Display(Name = "Unloading location")]
        public Address UnloadingLocation { get; set; }

        [Required]
        [Display(Name = "Delivery content")]
        public string DeliveryContent { get; set; }

        [Required]
        [Display(Name = "Loading date and time")]
        public DateTime LoadingDateTime { get; set; }

        [Required]
        [Display(Name = "Unloading date and time")]
        [DateGreaterThan("LoadingDateTime")]
        public DateTime UnloadingDateTime { get; set; }

        [ForeignKey("Driver")]
        public string? DriverId { get; set; }
        public Driver? Driver { get; set; }

        [ForeignKey("Truck")]
        public int? TruckId { get; set; }
        public Truck? Truck { get; set; }

        [ForeignKey("User")]
        public string? UserId { get; set; }
        public User? User { get; set; }

        public DeliveryStatus Status { get; set; }
        [Display(Name = "Commentaires")]
        public string? Comments { get; internal set; }
        public string? FailedReason { get; internal set; }

        [Display(Name = "Client")]
        public Client? Client { get; set; }
    }

    public enum DeliveryStatus
    {
        Pending,
        Assigned,
        Completed,
        Failed
    }
}

public class DateGreaterThanAttribute : ValidationAttribute
{
    private readonly string _comparisonProperty;

    public DateGreaterThanAttribute(string comparisonProperty)
    {
        _comparisonProperty = comparisonProperty;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var currentValue = (DateTime)value;

        var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

        if (property == null)
            throw new ArgumentException("Property with this name not found");

        var comparisonValue = (DateTime)property.GetValue(validationContext.ObjectInstance);

        if (currentValue <= comparisonValue)
            return new ValidationResult(ErrorMessage = "Date et heure de déchargement doit être supérieure à la date et heure de chargement");

        return ValidationResult.Success;
    }
}

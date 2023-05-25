using HELMo_bilite.Models;

namespace HELMo_bilite.ViewModels.Deliveries
{
    public class DeliveryViewModel
    {
        public int Id { get; set; }
        public int LoadingLocationId { get; set; }
        public Address LoadingLocation { get; set; }
        public int UnloadingLocationId { get; set; }
        public Address UnloadingLocation { get; set; }
        public string DeliveryContent { get; set; }
        public DateTime LoadingDateTime { get; set; }
        public DateTime UnloadingDateTime { get; set; }
        public string? DriverId { get; set; }
        public Driver? Driver { get; set; }
        public int? TruckId { get; set; }
        public Truck? Truck { get; set; }
        public string? UserId { get; set; }
        public User? User { get; set; }
        public DeliveryStatus Status { get; set; }
        public string? Comments { get; set; }
        public string? FailedReason { get; set; }
        public bool Tagged { get; set; }
    }

}

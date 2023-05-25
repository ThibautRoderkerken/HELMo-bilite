using HELMo_bilite.Models;

namespace HELMo_bilite.ViewModels.Deliveries
{
    internal class AdminGraphiqueViewModel
    {
        public IEnumerable<IGrouping<string, Delivery>> DeliveriesByDriver { get; set; }
        public IEnumerable<IGrouping<DateTime, Delivery>> DeliveriesByDate { get; set; }
        public IEnumerable<IGrouping<Client, Delivery>> DeliveriesByClient { get; set; }
        public List<Delivery> Deliveries { get; set; }
    }
}
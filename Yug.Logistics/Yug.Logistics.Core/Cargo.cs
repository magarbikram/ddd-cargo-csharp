namespace Yug.Logistics.Core
{
    public class Cargo
    {
        public string TrackingId { get; }
        public DeliveryHistory DeliveryHistory { get; }
        public Dictionary<CustomerRole, Customer> CustomerRoles { get; }

        public Cargo(string id)
        {
            TrackingId = id;
            CustomerRoles = new Dictionary<CustomerRole, Customer>();
        }

        public async Task<DeliveryHistory> GetDeliveryHistoryAsync(IHandlingEventRepository eventRepository)
        {
            return await new DeliveryHistory(this).FillEventsWith(eventRepository);
        }
        public Cargo CopyPrototype(string newTrackingId)
        {
            throw new NotSupportedException();
        }

        public Cargo NewCargo(Cargo prototype, string newTrackingId)
        {
            throw new NotSupportedException();
        }

        public Cargo NewCargo(Cargo prototype)
        {
            throw new NotSupportedException();
        }
    }
}
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
            DeliveryHistory = new DeliveryHistory(this);
            CustomerRoles = new Dictionary<CustomerRole, Customer>();
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
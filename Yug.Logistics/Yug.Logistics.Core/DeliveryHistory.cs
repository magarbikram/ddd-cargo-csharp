namespace Yug.Logistics.Core
{
    public class DeliveryHistory
    {
        public Cargo Cargo { get; }
        private List<HandlingEvent> _handlingEvents;
        public IReadOnlyCollection<HandlingEvent> HandlingEvents => _handlingEvents.AsReadOnly();
        public DeliveryHistory(Cargo cargo)
        {
            Cargo = cargo;
            _handlingEvents = new List<HandlingEvent>();
        }

        public void Add(HandlingEvent handlingEvent)
        {
            _handlingEvents.Add(handlingEvent);
        }
    }
}
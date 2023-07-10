namespace Yug.Logistics.Core
{
    /// <summary>
    /// even though it's an entity, we don't have to persist anything in database
    /// </summary>
    public class DeliveryHistory
    {
        public Cargo Cargo { get; }
        private List<HandlingEvent> _handlingEvents;
        public IReadOnlyCollection<HandlingEvent> HandlingEvents => _handlingEvents.AsReadOnly();
        public DeliveryHistory(Cargo cargo)
        {
            Cargo = cargo;
        }

        public async Task<DeliveryHistory> FillEventsWith(IHandlingEventRepository eventRepository)
        {
            _handlingEvents = (await eventRepository.FindByCargoTrackingIdAsync(Cargo.TrackingId)).ToList();
            return this;
        }
    }
}
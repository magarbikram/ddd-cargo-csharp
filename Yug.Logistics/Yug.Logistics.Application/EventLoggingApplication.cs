using Yug.Logistics.Core;

namespace Yug.Logistics.Application
{
    public class EventLoggingApplication
    {
        public void AddHandlingEvent(Cargo cargo, CarrierMovement loadedOnto)
        {
            HandlingEvent handlingEvent = HandlingEvent.NewLoading(cargo, loadedOnto, DateTimeOffset.Now);
            DeliveryHistory deliveryHistory = cargo.DeliveryHistory;

            //Transaction could fail because of contention for cargo and its component
            //delivery history
            deliveryHistory.Add(handlingEvent);
        }
    }
}
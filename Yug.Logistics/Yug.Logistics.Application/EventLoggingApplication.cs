using Yug.Logistics.Core.Shippping;

namespace Yug.Logistics.Application
{
    public class EventLoggingApplication
    {
        private readonly IHandlingEventRepository _handlingEventRepository;

        public EventLoggingApplication(IHandlingEventRepository handlingEventRepository)
        {
            _handlingEventRepository = handlingEventRepository;
        }
        public void AddHandlingEvent(Cargo cargo, CarrierMovement loadedOnto)
        {
            HandlingEvent handlingEvent = HandlingEvent.NewLoading(cargo, loadedOnto, DateTimeOffset.Now);
            _handlingEventRepository.Add(handlingEvent);
            //save changes to database
        }
    }
}
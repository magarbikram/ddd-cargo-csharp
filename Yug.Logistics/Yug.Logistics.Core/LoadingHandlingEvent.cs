namespace Yug.Logistics.Core
{
    public class LoadingHandlingEvent : HandlingEvent
    {
        private static readonly string _loadingEvent = "LoadingEvent";

        public CarrierMovement LoadedOnto { get; private set; }
        public LoadingHandlingEvent(Cargo cargo, CarrierMovement loadedOnto, DateTimeOffset timeStamp) : base(cargo, _loadingEvent, timeStamp)
        {
            LoadedOnto = loadedOnto;
        }

    }
}
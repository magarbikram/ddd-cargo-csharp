using System.ComponentModel;

namespace Yug.Logistics.Core.Shippping
{
    public class HandlingEvent
    {
        public Cargo Handled { get; }
        public string Type { get; }
        public DateTimeOffset CompletionTime { get; }

        protected HandlingEvent(Cargo cargo, string eventType, DateTimeOffset timeStamp)
        {
            Handled = cargo;
            Type = eventType;
            CompletionTime = timeStamp;
        }

        public static HandlingEvent NewLoading(Cargo cargo, CarrierMovement loadedOnto, DateTimeOffset timeStamp)
        {
            LoadingHandlingEvent result = new LoadingHandlingEvent(cargo, loadedOnto, timeStamp);
            return result;
        }
    }
}
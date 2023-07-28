using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yug.Logistics.Core.Shippping
{
    public interface IHandlingEventRepository
    {
        void Add(HandlingEvent handlingEvent);
        Task<IEnumerable<HandlingEvent>> FindByCargoIdTimeAndTypeAsync(string cargoTrackingId, DateTimeOffset timeStamp, string eventType);
        Task<IEnumerable<HandlingEvent>> FindByCargoTrackingIdAsync(string cargoTrackingId);
        Task<IEnumerable<HandlingEvent>> FindByScheduleIdAsync(string scheduleId);
        Task<HandlingEvent> FindMostRecentByCargoIdAndType(string cargoTrackingId, string eventType);

    }
}

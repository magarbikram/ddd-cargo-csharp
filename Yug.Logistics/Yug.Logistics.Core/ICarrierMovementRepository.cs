using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yug.Logistics.Core
{
    public interface ICarrierMovementRepository
    {
        CarrierMovement FindByScheduleId(string scheduleId);
        CarrierMovement FindByFromTo(Location from, Location to);
    }
}

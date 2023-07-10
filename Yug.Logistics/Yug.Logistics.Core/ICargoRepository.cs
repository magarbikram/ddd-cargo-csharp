using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yug.Logistics.Core
{
    public interface ICargoRepository
    {
        Cargo FindByCargoTrackingId(string cargoTrackingId);
        Cargo FindByCustomerId(string customerId);
    }
}

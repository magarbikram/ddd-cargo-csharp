using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yug.Logistics.Core
{
    public interface ICustomerRepository
    {
        Customer FindByCustomerId(string customerId);
        Customer FindByCustomerName(string customerName);
        Customer FindByCargoTrackingId(string cargoTrackingId);
    }
}

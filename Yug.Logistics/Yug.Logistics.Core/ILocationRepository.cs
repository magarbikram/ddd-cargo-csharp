using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yug.Logistics.Core
{
    public interface ILocationRepository
    {
        Location FindByCode(string code);
        Location FindByCityName(string cityName);
    }
}

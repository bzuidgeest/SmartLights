using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLights
{
    public interface ISmartLightController : IDictionary<object, ISmartLight>
    {
        
    }
}

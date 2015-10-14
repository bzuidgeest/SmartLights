using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLights
{
    [Flags]
    public enum SmartLightCapabilities
    {
        RGB,
        RGBW,
        White,
        RGBWI,
        RGBI
    }
}

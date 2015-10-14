using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLights
{
    public interface ISmartLight
    {
        SmartLightColor Color { get; }
        SmartLightColor PreviousColor { get; }

        SmartLightCapabilities GetCapabilities { get; }

        bool SetRGB(SmartLightColor color);

        bool SetRGBW(SmartLightColor color);

        bool SetWhite(SmartLightColor color);

        bool TurnOn();

        bool TurnOff();
    }
}

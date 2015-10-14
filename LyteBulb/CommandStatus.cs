using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLights.Lyte
{
    public enum CommandStatus
    {
        Idle = 0x00,
        ToBeSend = 0x01,
        Send = 0x02,
        Complete = 0x03
    }
}

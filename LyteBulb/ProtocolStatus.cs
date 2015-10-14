using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLights.Lyte
{
    public enum ProtocolStatus
    {
        Init = 0x00,
        Idle = 0x01,
        SendCommand = 0x02,
        ReceiveAckAndAnswer = 0x03
    }
}

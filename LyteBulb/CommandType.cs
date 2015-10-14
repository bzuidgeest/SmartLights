using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLights.Lyte
{
    public enum CommandType : byte
    {
        NoCommandSend = 0x0,  //                 bit[4-0] -> Command number
        SwitchOn = 0x1,
        SwitchOff = 0x2,
        SetRGB = 0x3,
        Brightness = 0x4,
        AddAddress = 0x5,
        DeleteAddress = 0x6,
        Reset = 0x7,
        ReadAddressTable = 0x8,
        SaveSettings = 0x9,
        InfoStatus = 0xA,
        SetSyncWord = 0xB,
        SetChannel = 0xC
    }
}

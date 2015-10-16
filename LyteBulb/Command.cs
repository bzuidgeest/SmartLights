using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SmartLights.Lyte
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Command
    {
        // Marco of authometion says commandtype enum should be two bytes
        public CommandType commandType;
        public byte destinationAddress1;
        public byte destinationAddress2;
        public byte frameNumber;
        public byte param1;
        public byte param2;
        public byte param3;
        public byte param4;
        public byte crc;
        // extra byte to compensate for bug in lyte lamp firmware
        public byte bugByte;

        public void CalculateCRC()
        {
            crc = 0;
            crc ^= (byte)commandType;
            // this is always zero with current commands but included for completenes. Zero has no effect on XOR.
            crc ^= (byte)((short)commandType >> 8);
            crc ^= destinationAddress1;
            crc ^= destinationAddress2;
            crc ^= frameNumber;
            crc ^= param1;
            crc ^= param2;
            crc ^= param3;
            crc ^= param4;
            //crc ^= unknown;
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append(commandType.ToString());
            result.Append(',');
            result.Append(destinationAddress1.ToString());
            result.Append(',');
            result.Append(destinationAddress2.ToString());
            result.Append(',');
            result.Append(frameNumber.ToString());
            result.Append(',');
            result.Append(param1.ToString());
            result.Append(',');
            result.Append(param2.ToString());
            result.Append(',');
            result.Append(param3.ToString());
            result.Append(',');
            result.Append(param4.ToString());
            result.Append(',');
            result.Append(crc.ToString());
            result.Append(',');
            result.Append(bugByte.ToString());
            return result.ToString();
        }
    }
}

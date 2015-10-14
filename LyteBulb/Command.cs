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
        public CommandType commandType;
        public byte destinationAddress1;
        public byte destinationAddress2;
        public byte frameNumber;
        public byte param1;
        public byte param2;
        public byte param3;
        public byte param4;
        public byte crc;
        public byte unknown;

        public void CalculateCRC()
        {
            crc = 0;
            crc ^= (byte)commandType;
            crc ^= destinationAddress1;
            crc ^= destinationAddress2;
            crc ^= frameNumber;
            crc ^= param1;
            crc ^= param2;
            crc ^= param3;
            crc ^= param4;
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
            result.Append(unknown.ToString());
            return result.ToString();
        }
    }
}

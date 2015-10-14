using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLights.Lyte
{
    public struct Answer
    {
        public CommandType answerToCommandType;
        public byte sourceAddress1;
        public byte sourceAddress2;
        public byte answerType;
        public byte answerToFrameNumber;
        public byte[] answer; //[Constants.PROTOCOL_ANSWER_LENGHT];
        public byte crc;

        public override string ToString()
        {
            return base.ToString();
        }
    }
}

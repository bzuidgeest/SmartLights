using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Radio.PL1167
{
    [Flags]
    public enum PL1167Status
    {
        ALL = 0xFFE0,
        CRC_ERR = 0x8000,
        FEC23_ERROR = 0x4000,
        FRAME_STATUS = 0x3F00,
        SYNCWORD_RECV = 0x0080,
        PKT_FLAG = 0x0040,
        FIFO_FLAG = 0x0020
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Radio.PL1167
{
    /// <summary>
    /// Register 0x07
    /// 15:9 Reserved
    /// 8 EN_TX Active high
    /// 7 EN RX Active high
    /// 6:0 RF_CH_SEL
    /// </summary>
    public enum PL1167Mode
    {
        Transmitting = 256, //0x100 
        Receiving = 128, // 0x80
        Disabled = 0
    };
}

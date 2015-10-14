using System.Threading;

namespace Radio.PL1167
{
    public abstract class PL1167
    {
        protected PL1167Mode mode;
        public PL1167Mode Mode { get { return mode; } }

        protected byte channel = 33;
        public byte Channel { get { return channel; } }

        public byte lastRSSIRead;
        public byte LastRSSIRead { get { return lastRSSIRead; } }

        public PL1167Status lastRegStatusRead;
        public PL1167Status LastRegStatusRead { get { return lastRegStatusRead; } }

        // Constants

        //private const int PL1167_DEFAULT_RADIO_TRASMISSION = 33;
        protected static int PL1167_BUFFER_LENGTH = 25;
        protected const int PL1167_PKT_PIN = 2;

        protected const int C_REG_RD = 0x80;

        protected const int C_TXFIFOPOINTER = 0x8000;
        protected const int C_RXFIFOPOINTER = 0x0080;

        //private const int C_RXMODE = 0x00;
        //private const int C_TXMODE = 0x01;

        //default sync word
        protected const int C_MSBYTE_SYNCWORD0 = 66;
        protected const int C_LSBYTE_SYNCWORD0 = 49;



        // Private variables
        //protected byte CS_Pin;                  //CS pin for PL1167 radio module
        //protected byte RadioTransmission;       //Number of radio transmission
        //protected byte MSByte, LSByte, DeviceType;

        //public variables
        public bool RXBufferAvailable;
        public byte RXBufferLength;
        public char[] TXBuffer = new char[PL1167_BUFFER_LENGTH];
        public char[] RXBuffer = new char[PL1167_BUFFER_LENGTH];
        

        protected abstract void WriteRegister16(byte ADDR, byte MSByte, byte LSByte, byte WAIT);

        protected abstract int ReadRegister16(byte ADDR, byte WAIT);

        public abstract void WriteTXFIFO(byte[] transmitBuffer);

        public abstract byte[] ReadRXFIFO();


        /// <summary>
        /// Initialize registers of PL1167
        /// </summary>
        protected void InitRadioModule()
        {
            WriteRegister16(0, 111, 224, 7);
            WriteRegister16(1, 86, 129, 7);
            WriteRegister16(2, 102, 23, 7);
            WriteRegister16(4, 156, 201, 7);
            WriteRegister16(5, 102, 55, 7);
            WriteRegister16(7, 0, 76, 7);
            WriteRegister16(8, 108, 144, 7);
            WriteRegister16(9, 72, 0, 7);
            WriteRegister16(10, 127, 253, 7);
            WriteRegister16(11, 0, 8, 7);
            WriteRegister16(12, 0, 0, 7);
            WriteRegister16(13, 72, 189, 7);
            WriteRegister16(22, 0, 255, 7);
            WriteRegister16(23, 128, 5, 7);
            WriteRegister16(24, 0, 103, 7);
            WriteRegister16(25, 22, 89, 7);
            WriteRegister16(26, 25, 224, 7);
            WriteRegister16(27, 19, 0, 7);
            WriteRegister16(28, 24, 0, 7);
            WriteRegister16(32, 88, 0, 7);
            WriteRegister16(33, 63, 199, 7);
            WriteRegister16(34, 32, 0, 7);
            WriteRegister16(35, 3, 0, 7);
            // Start Syncwords
            WriteRegister16(36, C_MSBYTE_SYNCWORD0, C_LSBYTE_SYNCWORD0, 7);
            WriteRegister16(37, 137, 117, 7);
            WriteRegister16(38, 154, 11, 7);
            WriteRegister16(39, 222, 199, 7);
            // End Syncwords
            WriteRegister16(40, 68, 2, 7);
            WriteRegister16(41, 176, 0, 7);
            WriteRegister16(42, 253, 176, 7);
            WriteRegister16(43, 0, 15, 7);
            //#if !defined(__OPTIMIZE_CODE__)
            Thread.Sleep(200);
            WriteRegister16(128, 0, 0, 7);
            WriteRegister16(129, 255, 255, 7);
            WriteRegister16(130, 0, 0, 7);
            WriteRegister16(132, 0, 0, 7);
            WriteRegister16(133, 255, 255, 7);
            WriteRegister16(135, 255, 255, 7);
            WriteRegister16(136, 0, 0, 7);
            WriteRegister16(137, 255, 255, 7);
            WriteRegister16(138, 0, 0, 7);
            WriteRegister16(139, 255, 255, 7);
            WriteRegister16(140, 0, 0, 7);
            WriteRegister16(141, 255, 255, 7);
            WriteRegister16(150, 0, 0, 7);
            WriteRegister16(151, 255, 255, 7);
            WriteRegister16(152, 0, 0, 7);
            WriteRegister16(153, 255, 255, 7);
            WriteRegister16(154, 0, 0, 7);
            WriteRegister16(155, 255, 255, 7);
            WriteRegister16(156, 0, 0, 7);
            WriteRegister16(160, 0, 0, 7);
            WriteRegister16(161, 255, 255, 7);
            WriteRegister16(162, 0, 0, 7);
            WriteRegister16(163, 255, 255, 7);
            WriteRegister16(168, 0, 0, 7);
            WriteRegister16(169, 255, 255, 7);
            WriteRegister16(170, 0, 0, 7);
            WriteRegister16(171, 255, 255, 7);
            WriteRegister16(7, 0, 0, 7);
            //#endif
        }

        /// <summary>
        /// Set PL1167 in TX mode 
        /// Register 0x07 -> Transmit 0x01 
        /// </summary>
        protected void SetTXMode()
        {
            WriteRegister16(0x07, 0x01, (byte)(channel), 1);
            mode = PL1167Mode.Transmitting;
        }

        // <summary>
        /// Set PL1167 in RX mode 
        /// Register 0x07 -> Transmit 0x00
        /// </summary>
        protected void SetRXMode()
        {
            WriteRegister16(0x07, 0x00, (byte)(0x80 + channel), 1);
            mode = PL1167Mode.Receiving;
        }

        // <summary>
        /// Switch off TX and RX mode of PL1167 
        /// </summary>
        protected void StopRXTXMode()
        {
            WriteRegister16(0x07, 0x00, channel, 1);
            mode = PL1167Mode.Disabled;
        }

        protected PL1167Mode GetRXTXMode()
        {
            return (PL1167Mode)(ReadRegister16(0x07, 1) - channel);
        }


        // <summary>
        /// Reset TX or RX FIFO register of PL1167 
        /// </summary>
        protected void ResetFIFOPointerReg(ushort ui8FIFOToReset)
        {
            WriteRegister16(0x34, (byte)(ui8FIFOToReset >> 8), (byte)(ui8FIFOToReset), 1);  // FIFO Pointer Register
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">A value from 0 to 127</param>
        /// <returns></returns>
        public bool SetRadioChannel(int channel)
        {
            if (channel < 128)
            {
                int register = ReadRegister16(0x07, 1);  // TX/RX Enable and Channel Register
                register = register & 0x180; // 0b0000000110000000 Blank channel portion
                register = register + channel;
                WriteRegister16(0x07, (byte)((register >> 8) & 0x1), (byte)(register & 0xFF), 7);      // TX/RX Enable and Channel Register
                return true;
            }
            else
                return false;
        }

        public int GetRadioChannel()
        {
            int register = ReadRegister16(0x07, 1);  // TX/RX Enable and Channel Register
            return register & 0x7F; // 0b0000 0001 1000 0000 Blank TX/RX portion
        }

        public void SetSyncWords(ushort SyncWord0)
        {
            WriteRegister16(36, (byte)(SyncWord0 >> 8), (byte)(SyncWord0), 7);
            //WriteRegister16(37, (byte)(137), (byte)117, 7);
            //WriteRegister16(38, (byte)(154), (byte)(11), 7);
            //WriteRegister16(39, (byte)(222), (byte)(199), 7);
        }

        public void SetSyncWords(ushort SyncWord0, ushort SyncWord1, ushort SyncWord2, ushort SyncWord3)
        {
            WriteRegister16(36, (byte)(SyncWord0 >> 8), (byte)(SyncWord0), 7);
            WriteRegister16(37, (byte)(SyncWord1 >> 8), (byte)(SyncWord1), 7);
            WriteRegister16(38, (byte)(SyncWord2 >> 8), (byte)(SyncWord2), 7);
            WriteRegister16(39, (byte)(SyncWord3 >> 8), (byte)(SyncWord3), 7);
        }

        public int GetSyncWord0()
        {
            return ReadRegister16(36, 7);
            //WriteRegister16(37, (byte)(137), (byte)117, 7);
            //WriteRegister16(38, (byte)(154), (byte)(11), 7);
            //WriteRegister16(39, (byte)(222), (byte)(199), 7);
        }

        /// <summary>
        /// Read PL1167 status register
        /// </summary>
        /// <returns>Status register flags</returns>
        public PL1167Status ReadStatusReg()
        {
            lastRegStatusRead = (PL1167Status)ReadRegister16(0x30, 7);
            return lastRegStatusRead;
        }

        // RX and TX ISR
        public void vfPKT_ISR() { }

    }
}
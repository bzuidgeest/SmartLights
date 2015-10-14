using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BusPirateLibCS;
using BusPirateLibCS.Modes;

namespace Radio.PL1167
{
    public class BPPL1167 : PL1167
    {

        private BusPirate busPirate;
        private Spi spiConnection;

        // Class constructor
        public BPPL1167()
        {
            lastRSSIRead = 0;
            RXBufferLength = 0;
            RXBufferAvailable = false;
        }

        ~BPPL1167()
        {
            //if (spiConnection != null)
            //    spiConnection.Power = false;
            //if (spiConnection != null)
            //    spiConnection.ExitMode();

            //if (busPirate != null)
            //    busPirate.Close();
        }

        #region protected functions

        /// <summary>
        /// Write a register of PL1167
        /// </summary>
        /// <param name="ADDR">Register address to write int16 to</param>
        /// <param name="MSByte">Most significant byte of value\</param>
        /// <param name="LSByte">Least significant byte of value</param>
        /// <param name="WAIT">Delay after write</param>
        protected override void WriteRegister16(byte ADDR, byte MSByte, byte LSByte, byte WAIT)
        {
            // Create new buffer of one byte.
            spiConnection.CS = false;
            byte[] response = spiConnection.WriteBulk(new byte[] { ADDR, MSByte, LSByte});
            // Create new buffer of two bytes. Read them. Not setting Tx data shifts out zero
            spiConnection.CS = true;
            Thread.Sleep(WAIT);
        }
        
        /// <summary>
        /// Read a register of PL1167
        /// </summary>
        /// <param name="ADDR">Address of register</param>
        /// <param name="WAIT">Delay time after read</param>
        /// <returns></returns>
        protected override int ReadRegister16(byte ADDR, byte WAIT)
        {
            spiConnection.CS = false;
            byte[] response = spiConnection.WriteBulk(new byte[] { (byte)(ADDR | C_REG_RD), 0x00, 0x00 });
            spiConnection.CS = true;

            Thread.Sleep(WAIT);

            return (response[1] << 8) + response[2];
        }

        
        #endregion

        #region public functions

        /// <summary>
        /// Initialization function for PL1167
        /// 
        /// </summary>
        /// <param name="ui8CS"></param>
        public void Initialize(string portName)
        {
            //init SPI
            busPirate = new BusPirate(new System.IO.Ports.SerialPort(portName, 115200));
            busPirate.Open();
            spiConnection = new Spi(busPirate);
            spiConnection.EnterMode();
            spiConnection.SpeedMode = Spi.Speed.s250khz;
            spiConnection.ConfigProtocol(true, false, true, true);
            //Enable Power, PullUps
            // sample on END. Middle gives incorrect value
            spiConnection.ConfigPins(true, false, false, true);
            

            Thread.Sleep(1000);
            //Init PL1167
#if PL1167_EXPLICIT_RESET
            //spiConnection.AUX = true;
            //Thread.Sleep(50);
            spiConnection.AUX = false;
            Thread.Sleep(50);
            spiConnection.AUX = true;
#endif
            // Delay for oscillator locking
            Thread.Sleep(10);//10
            
            //pinMode(PL1167_PKT_PIN, INPUT);
            //attachInterrupt(0, vfISR, RISING);                  // Initialize Arduino interrupt
            InitRadioModule();                                // Initialize registers of PL1167
            
            Thread.Sleep(50);
            SetRadioChannel(channel);  // Set RF channel 

            ResetFIFOPointerReg(C_RXFIFOPOINTER);
            SetRXMode();
        }

        // Read and write RX/TX FIFO
        public override void WriteTXFIFO(byte[] transmitBuffer)
        {
            if (transmitBuffer.Length > 255)
                throw new Exception("Max buffer length is 256");
            //test hack;
            //transmitBuffer = new byte[] { 130, 0, 0, 0, 0, 0, 0, 130, 0};

            if (mode != PL1167Mode.Disabled)
                StopRXTXMode();

            ResetFIFOPointerReg(C_TXFIFOPOINTER);

            spiConnection.CS = false;

            byte[] buffer = new byte[transmitBuffer.Length + 3];
            buffer[0] = 0x32;
            buffer[1] = (byte)(transmitBuffer.Length + 1);
            //buffer[1] = (byte)(transmitBuffer.Length);
            for (int i = 0; i < transmitBuffer.Length; i++)
            { 
                buffer[i + 2] = transmitBuffer[i];
            }

            byte[] result = spiConnection.WriteBulk(buffer);

            spiConnection.CS = true;

            Thread.Sleep(10);

            SetTXMode();

            PL1167Status status = ReadStatusReg();
            //while (digitalRead(PL1167_PKT_PIN) == 0) ;
            //	delayMicroseconds(400);
            //byte[] test = ReadRXFIFO();
        }
        
        public override byte[] ReadRXFIFO()
        {
            lastRSSIRead = (byte)(ReadRegister16(0x06, 7) >> 10);

            spiConnection.CS = false;

            byte[] readStart = spiConnection.WriteBulk(new byte[] { (byte)(0x32 | C_REG_RD), 0x00 });

            byte[] message;

            if (readStart[1] > 0)
                message = spiConnection.WriteBulk(new byte[readStart[1]]);
            else
                message = new byte[0];

            spiConnection.CS = true;

            ReadStatusReg();

            return message;
        }

        

        #endregion
    }
}

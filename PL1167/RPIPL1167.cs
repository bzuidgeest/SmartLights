using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Raspberry.IO.SerialPeripheralInterface;
using Raspberry.IO.Interop;

namespace Radio.PL1167
{
    public class RPIPL1167 : PL1167
    {

        NativeSpiConnection spiConnection;

        // Class constructor
        public RPIPL1167()
        {
            lastRSSIRead = 0;
            RXBufferLength = 0;
            RXBufferAvailable = false;
        }

        #region private functions
       
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
            SpiTransferBuffer addressBuffer = new SpiTransferBuffer(1, SpiTransferMode.Write) { ChipSelectChange = true };
            addressBuffer.Tx[0] = ADDR;
            spiConnection.Transfer(addressBuffer);

            // Create new buffer of two bytes. Read them. Not setting Tx data shifts out zero
            SpiTransferBuffer readBuffer = new SpiTransferBuffer(2, SpiTransferMode.Write) { ChipSelectChange = true };
            addressBuffer.Tx[0] = MSByte;
            addressBuffer.Tx[1] = LSByte;
            spiConnection.Transfer(readBuffer);

            //delayMicroseconds(WAIT);
        }
        
        /// <summary>
        /// Read a register of PL1167
        /// </summary>
        /// <param name="ADDR">Address of register</param>
        /// <param name="WAIT">Delay time after read</param>
        /// <returns></returns>
        protected override int ReadRegister16(byte ADDR, byte WAIT)
        {
            // Create new buffer of one byte.
            SpiTransferBuffer addressBuffer = new SpiTransferBuffer(1, SpiTransferMode.Write) { ChipSelectChange = true };
            addressBuffer.Tx[0] = (byte)(ADDR | C_REG_RD);
            spiConnection.Transfer(addressBuffer);

            // Create new buffer of two bytes. Read them. Not setting Tx data shifts out zero
            SpiTransferBuffer readBuffer = new SpiTransferBuffer(2, SpiTransferMode.Read) { ChipSelectChange = true };
            spiConnection.Transfer(readBuffer);

            return readBuffer.Rx[0] << 8 + readBuffer.Rx[1];

            //delayMicroseconds(WAIT);
        }

        
        
        #endregion

        #region public functions

        /// <summary>
        /// Initialization function for PL1167
        /// 
        /// </summary>
        /// <param name="ui8CS"></param>
        public void Initialize(byte ui8CS)
        {
            //CS_Pin = ui8CS;

            //Init SPI 
            //pinMode(ui8CS_Pin, OUTPUT);
            spiConnection = new NativeSpiConnection("/dev/spidev0.0", new SpiConnectionSettings() {
                Mode = SpiMode.Mode1              
            });
            
            //Init PL1167
#if PL1167_EXPLICIT_RESET
            //pinMode(PL1167_EXPLICIT_RESET_PIN, OUTPUT);
            //digitalWrite(PL1167_EXPLICIT_RESET_PIN, LOW);
            //delay(50);
            //digitalWrite(PL1167_EXPLICIT_RESET_PIN, HIGH);
#endif
            //delay(10);                                          // Delay for oscillator locking
            //pinMode(PL1167_PKT_PIN, INPUT);
            //attachInterrupt(0, vfISR, RISING);                  // Initialize Arduino interrupt
            InitRadioModule();                                // Initialize registers of PL1167
            //delay(50);
            SetRadioChannel(channel);  // Set RF channel 

            ResetFIFOPointerReg(C_RXFIFOPOINTER);
            SetRXMode();
        }

        // Read and write RX/TX FIFO
        public override void WriteTXFIFO(byte[] transmitBuffer)
        {
            if (transmitBuffer.Length < 256)
                throw new Exception("Max buffer length is 256");

            if (mode != PL1167Mode.Disabled)
                StopRXTXMode();

            ResetFIFOPointerReg(C_TXFIFOPOINTER);

            SpiTransferBuffer addressBuffer = new SpiTransferBuffer(1, SpiTransferMode.Write) { ChipSelectChange = true };
            addressBuffer.Tx[0] = 0x32;
            spiConnection.Transfer(addressBuffer);

            //First byte contains FIFO length
            SpiTransferBuffer dataBuffer = new SpiTransferBuffer(transmitBuffer.Length + 1, SpiTransferMode.Write) { ChipSelectChange = true };
            addressBuffer.Tx[0] = (byte)transmitBuffer.Length;
            for (int i = 1; i < transmitBuffer.Length + 1; i++)
                addressBuffer.Tx[i] = transmitBuffer[i];
            spiConnection.Transfer(addressBuffer);
            
            //delayMicroseconds(10);
            SetTXMode();

            ReadStatusReg();
            //while (digitalRead(PL1167_PKT_PIN) == 0) ;
            //	delayMicroseconds(400);
            
        }


        public override byte[] ReadRXFIFO()
        {
            lastRSSIRead = (byte)(ReadRegister16(0x06, 7) >> 10);

            SpiTransferBuffer addressBuffer = new SpiTransferBuffer(1, SpiTransferMode.Write) { ChipSelectChange = true };
            addressBuffer.Tx[0] = (0x32 | C_REG_RD); // Start reading of FIFO Data Register
            spiConnection.Transfer(addressBuffer);

            SpiTransferBuffer dataLengthBuffer = new SpiTransferBuffer(1, SpiTransferMode.Read) { ChipSelectChange = true };
            spiConnection.Transfer(dataLengthBuffer);

            int receiveBufferLength = dataLengthBuffer.Rx[0]; // First byte of FIFO Data Register is the length of message to receive
            receiveBufferLength = (receiveBufferLength <= PL1167_BUFFER_LENGTH) ? receiveBufferLength : PL1167_BUFFER_LENGTH;  // First byte of FIFO Data Register is the length of message to receive

            SpiTransferBuffer dataBuffer = new SpiTransferBuffer(receiveBufferLength, SpiTransferMode.Read) { ChipSelectChange = true };
            spiConnection.Transfer(dataLengthBuffer);

            byte[] receiveBuffer = new byte[receiveBufferLength];
            for (int i = 0; i < receiveBufferLength; i++)
                receiveBuffer[i] = dataBuffer.Rx[i];

            ReadStatusReg();

            return receiveBuffer;
        }
              

        #endregion
    }
}

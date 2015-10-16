using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLights.Lyte
{
    public class Lyte : ISmartLight
    {
        private SmartLightColor color;
        private SmartLightColor previousColor;
        private byte upperAddressByte;
        private byte lowerAddressByte;
        private TransmissionMode transmissionMode;
        private byte frameCounter = 1;

        public SmartLightColor Color
        {
            get
            {
                return color;
            }
        }

        public SmartLightColor PreviousColor
        {
            get
            {
                return previousColor;
            }
        }

        public short Address { get { return (short)(upperAddressByte << 8 + lowerAddressByte); } }

        public event EventHandler<CommandReadyEventArgs> OnCommandReady;

        public Lyte(short address, TransmissionMode transmissionMode)
        {
            this.lowerAddressByte = (byte)(address & 0xFF);
            this.upperAddressByte = (byte)(address >> 8 & 0xFF);
            this.transmissionMode = transmissionMode;
        }

        public SmartLightCapabilities GetCapabilities
        {
            get
            {
                return SmartLightCapabilities.RGBW;
            }
        }

        /**
        * description  It switches OFF the bulb 
        * param        address1   : high address of bulb [0..255]
        * param        address2   : low address of bulb [0..255]
        * param        castType   : transmission mode
        *                              This parameter can be one of following values:
        *                                Constants.C_UNICAST  : command sent to a single bulb and check of answer (default)
        *                                Constants.C_MULTICAST: command sent to a multiple bulbs and no check of answer
        * param        dimmerSteps: number of dimmer steps. One step every 10 ms.
        *                              This parameter can be one of following values:
        *                                0    : no dimmer effect (default)
        *                                value: dimmer effect in value*10 ms [1..100]
        * retval       none
        * note         In Constants.C_UNICAST mode, the bulb sends an answer for reporting command results
        */
        public bool TurnOff()
        {
            byte dimmerSteps = 0;
            Command commandToSend = new Command();

            commandToSend.commandType = (CommandType)((byte)transmissionMode | (byte)CommandType.SwitchOff);
            commandToSend.destinationAddress1 = upperAddressByte;
            commandToSend.destinationAddress2 = lowerAddressByte;

            if (transmissionMode == TransmissionMode.Unicast)
                commandToSend.frameNumber = frameCounter++;

            if (transmissionMode == TransmissionMode.Unicast)
                commandToSend.param1 = (byte)((dimmerSteps <= Constants.PROTOCOL_DIMMER_STEPS) ? dimmerSteps : Constants.PROTOCOL_DIMMER_STEPS);
            else
                commandToSend.param1 = 0;

            commandToSend.param2 = 0;
            commandToSend.param3 = 0;
            commandToSend.param4 = 0;

            commandToSend.CalculateCRC();


            if (OnCommandReady != null)
            {
                OnCommandReady(this, new CommandReadyEventArgs(commandToSend));
            }

            return true;
        }

        /**
        * description  It switches ON the bulb 
        * param        address1   : high address of bulb [0..255]
        * param        address2   : low address of bulb [0..255]
        * param        castType   : transmission mode
        *                              This parameter can be one of following values:
        *                                Constants.C_UNICAST  : command sent to a single bulb and check of answer (default)
        *                                Constants.C_MULTICAST: command sent to a multiple bulbs and no check of answer
        * param        dimmerSteps: number of dimmer steps. One step every 10 ms.
        *                              This parameter can be one of following values:
        *                                0    : no dimmer effect (default)
        *                                value: dimmer effect in value*10 ms [1..100]
        * retval       none
        * note         In Constants.C_UNICAST mode, the bulb sends an answer for reporting command results
        */
        public bool TurnOn()
        {
            byte dimmerSteps = 0;
            Command commandToSend = new Command();

            commandToSend.commandType = (CommandType)((byte)transmissionMode | (byte)CommandType.SwitchOn);
            commandToSend.destinationAddress1 = upperAddressByte;
            commandToSend.destinationAddress2 = lowerAddressByte;

            if (transmissionMode == TransmissionMode.Unicast)
            {
                commandToSend.frameNumber = frameCounter++;
                commandToSend.param1 = (byte)((dimmerSteps <= Constants.PROTOCOL_DIMMER_STEPS) ? dimmerSteps : Constants.PROTOCOL_DIMMER_STEPS);
            }
            else
                commandToSend.param1 = 0;

            commandToSend.param2 = 0;
            commandToSend.param3 = 0;
            commandToSend.param4 = 0;
            commandToSend.crc = 0;

            commandToSend.CalculateCRC();

            if (OnCommandReady != null)
            {
                OnCommandReady(this, new CommandReadyEventArgs(commandToSend));
            }

            return true;
        }

        /**
        * description  It sets RGB values of the bulb 
        * param        address1   : high address of bulb [0..255]
        * param        address2   : low address of bulb [0..255]
        * param        ui8RedValue   : PWM value of red coulor [0..255]
        * param        uiGreenValue  : PWM value of red coulor [0..255]
        * param        uiBlueValue   : PWM value of red coulor [0..255]
        * param        castType   : transmission mode
        *                              This parameter can be one of following values:
        *                                Constants.C_UNICAST  : command sent to a single bulb and check of answer (default)
        *                                Constants.C_MULTICAST: command sent to a multiple bulbs and no check of answer
        * param        dimmerSteps: number of dimmer steps. One step every 10 ms. 
        *                              This parameter can be one of following values:
        *                                0    : no dimmer effect (default)
        *                                value: dimmer effect in value*10 ms [1..100]
        * retval       none
        * note         In Constants.C_UNICAST mode, the bulb sends an answer for reporting command results
        */
        public bool SetRGB(SmartLightColor color)
        {
            byte dimmerSteps = 0;
            Command commandToSend = new Command();

            commandToSend.commandType = (CommandType)((byte)transmissionMode | (byte)CommandType.SetRGB);
            commandToSend.destinationAddress1 = upperAddressByte;
            commandToSend.destinationAddress2 = lowerAddressByte;
            
            commandToSend.param1 = color.red;
            commandToSend.param2 = color.green;
            commandToSend.param3 = color.blue;

            if (transmissionMode == TransmissionMode.Unicast)
            {
                commandToSend.param4 = (byte)((dimmerSteps <= Constants.PROTOCOL_DIMMER_STEPS) ? dimmerSteps : Constants.PROTOCOL_DIMMER_STEPS);
                commandToSend.frameNumber = frameCounter++;
            }
            else
                commandToSend.param4 = 0;

            commandToSend.CalculateCRC();

            if (OnCommandReady != null)
            {
                OnCommandReady(this, new CommandReadyEventArgs(commandToSend));
            }

            return true;
        }

        public bool SetRGBW(SmartLightColor color)
        {
            throw new NotImplementedException();
        }

        /**
        * description  It sets Brightness value of the bulb 
        * param        address1       : high address of bulb [0..255]
        * param        address2       : low address of bulb [0..255]
        * param        ui8BrightnessValue: PWM value of Brightness [0..255]
        * param        castType       : transmission mode
        *                                  This parameter can be one of following values:
        *                                    Constants.C_UNICAST  : command sent to a single bulb and check of answer (default)
        *                                    Constants.C_MULTICAST: command sent to a multiple bulbs and no check of answer
        * param        dimmerSteps    : number of dimmer steps. One step every 10 ms. 
        *                                  This parameter can be one of following values:
        *                                    0    : no dimmer effect (default)
        *                                    value: dimmer effect in value*10 ms [1..100]
        * retval       none
        * note         In Constants.C_UNICAST mode, the bulb sends an answer for reporting command results
        */
        public bool SetWhite(SmartLightColor color)
        {
            byte dimmerSteps = 0;
            Command commandToSend = new Command();

            commandToSend.commandType = (CommandType)((byte)transmissionMode | (byte)CommandType.Brightness);
            commandToSend.destinationAddress1 = upperAddressByte;
            commandToSend.destinationAddress2 = lowerAddressByte;
            
            commandToSend.param1 = color.white;

            if (transmissionMode == TransmissionMode.Unicast)
            {
                commandToSend.param2 = (byte)((dimmerSteps <= Constants.PROTOCOL_DIMMER_STEPS) ? dimmerSteps : Constants.PROTOCOL_DIMMER_STEPS);
                commandToSend.frameNumber = frameCounter++;
            }
            else
                commandToSend.param2 = 0;
            commandToSend.param3 = 0;
            commandToSend.param4 = 0;
            commandToSend.crc = 0;

            commandToSend.CalculateCRC();

            if (OnCommandReady != null)
            {
                OnCommandReady(this, new CommandReadyEventArgs(commandToSend));
            }

            return true;
        }

        #region Lyte specific function

        /**
        * description  It adds a new address to bulb
        * param        address1   : high address of bulb [0..255]
        * param        address2   : low address of bulb [0..255]
        * param        ui8NewAddress1: high address of bulb to add [0..255]
        * param        ui8NewAddress2: low address of bulb to add [0..255]
        * retval       none
        * note         The bulb sends an answer for reporting command results
                        This is always unicast.
        */
        void addLightAddress(byte newUpperAddressByte, byte newLowerAddressByte)
        {
            Command commandToSend = new Command();

            commandToSend.commandType = CommandType.AddAddress;
            commandToSend.destinationAddress1 = upperAddressByte;
            commandToSend.destinationAddress2 = lowerAddressByte;
            commandToSend.frameNumber = frameCounter++;
            commandToSend.param1 = newUpperAddressByte;
            commandToSend.param2 = newLowerAddressByte;
            commandToSend.param3 = 0;
            commandToSend.param4 = 0;

            commandToSend.CalculateCRC();

            if (OnCommandReady != null)
            {
                OnCommandReady(this, new CommandReadyEventArgs(commandToSend));
            }
        }

        /**
        * description  It deletes a address to bulb
        * param        address1   : high address of bulb [0..255]
        * param        address2   : low address of bulb [0..255]
        * param        ui8NewAddress1: high address of bulb to delete [0..255]
        * param        ui8NewAddress2: low address of bulb to delete[0..255]
        * retval       none
        * note         The bulb sends an answer for reporting command results
        */
        void DeleteLightAddress(byte addressToBeDeleted1, byte addressToBeDeleted2)
        {
            Command commandToSend = new Command();

            commandToSend.commandType = CommandType.DeleteAddress;
            commandToSend.destinationAddress1 = upperAddressByte;
            commandToSend.destinationAddress2 = lowerAddressByte;
            commandToSend.frameNumber = frameCounter++;
            commandToSend.param1 = addressToBeDeleted1;
            commandToSend.param2 = addressToBeDeleted2;
            commandToSend.param3 = 0;
            commandToSend.param4 = 0;

            commandToSend.CalculateCRC();

            if (OnCommandReady != null)
            {
                OnCommandReady(this, new CommandReadyEventArgs(commandToSend));
            }
        }

        /**
        * description  It resets the bulb to its default operative values
        * param        none
        * retval       none
        * note         This command must be sent within three seconds from power on of bulb
        * note         The bulb sends an answer for reporting command results
        */
        void ResetLight()
        {
            Command commandToSend = new Command();

            //SetLocalChannel(Constants.PL1167_DEFAULT_RADIO_TRASMISSION);
            //SetLocalSyncWord(Constants.C_MSBYTE_SYNCWORD0, Constants.C_LSBYTE_SYNCWORD0);
            commandToSend.commandType = CommandType.Reset;
            commandToSend.destinationAddress1 = Constants.C_INVALID_ADDRESS;
            commandToSend.destinationAddress2 = Constants.C_INVALID_ADDRESS;
            commandToSend.frameNumber = frameCounter++;
            commandToSend.param1 = 0;
            commandToSend.param2 = 0;
            commandToSend.param3 = 0;
            commandToSend.param4 = 0;
            commandToSend.crc = 0;

            commandToSend.CalculateCRC();

            if (OnCommandReady != null)
            {
                OnCommandReady(this, new CommandReadyEventArgs(commandToSend));
            }
        }


        /**
        * description  It reads the address table of bulb
        * param        address1   : high address of bulb [0..255]
        * param        address2   : low address of bulb [0..255]
        * retval       none
        * note         The bulb sends an answer for reporting command results
        * note         Results are available in LampAddressTable variavle
        */
        void ReadLightAddressTable()
        {
            Command commandToSend = new Command();

            commandToSend.commandType = CommandType.ReadAddressTable;
            commandToSend.destinationAddress1 = upperAddressByte;
            commandToSend.destinationAddress2 = lowerAddressByte;
            commandToSend.frameNumber = frameCounter++;
            commandToSend.param1 = 0;
            commandToSend.param2 = 0;
            commandToSend.param3 = 0;
            commandToSend.param4 = 0;

            commandToSend.CalculateCRC();

            if (OnCommandReady != null)
            {
                OnCommandReady(this, new CommandReadyEventArgs(commandToSend));
            }
        }

        /**
          * description  It saves the actual status (PWM values and ON/OFF condition) of bulb in EEPROM
          * param        address1   : high address of bulb [0..255]
          * param        address2   : low address of bulb [0..255]
          * retval       none
          * note         The bulb sends an answer for reporting command results
          */
        void SaveLampSettings()
        {
            Command commandToSend = new Command();

            commandToSend.commandType = CommandType.SaveSettings;
            commandToSend.destinationAddress1 = upperAddressByte;
            commandToSend.destinationAddress2 = lowerAddressByte;
            commandToSend.frameNumber = frameCounter++;
            commandToSend.param1 = 0;
            commandToSend.param2 = 0;
            commandToSend.param3 = 0;
            commandToSend.param4 = 0;

            commandToSend.CalculateCRC();

            if (OnCommandReady != null)
            {
                OnCommandReady(this, new CommandReadyEventArgs(commandToSend));
            }
        }

        /**
          * description  It reads the actual state of bulb
          * param        address1   : high address of bulb [0..255]
          * param        address2   : low address of bulb [0..255]
          * retval       none
          * note         The bulb sends an answer for reporting command results
          */
        void AskLampInfoStatus()
        {
            Command commandToSend = new Command();

            commandToSend.commandType = CommandType.InfoStatus;
            commandToSend.destinationAddress1 = upperAddressByte;
            commandToSend.destinationAddress2 = lowerAddressByte;
            commandToSend.frameNumber = 0;
            commandToSend.param1 = 0;
            commandToSend.param2 = 0;
            commandToSend.param3 = 0;
            commandToSend.param4 = 0;

            commandToSend.CalculateCRC();

            if (OnCommandReady != null)
            {
                OnCommandReady(this, new CommandReadyEventArgs(commandToSend));
            }
        }

        /**
          * description  It reads the actual state of bulb and it returns answer received from it
          * param        address1   : high address of bulb [0..255]
          * param        address2   : low address of bulb [0..255]
          * retval       Constants.C_ACK                          0x00  Bulb receives good command
          *              Constants.C_NACK                         0x01  Bulb receives corrupt command
          *              Constants.C_ERROR_COMMAND_IN_PROGRESS    0x06  Bulb's is busy to do a previous command
          *              Constants.C_ERROR_BAD_ANSWER_CRC         0x10  LYT shield receives corrupt answer
          *              Constants.C_ERROR_NO_ANSWER              0x11  LYT shield does not receive answer by Bulb
          *              Constants.C_ERROR_WRONG_ANSWER           0x12  LYT shield receives an answer not corresponding to sent command
          * note         The bulb sends an answer for reporting command results
          */
        //byte AskLampInfoStatusAndCheck(byte address1, byte address2)
        //{
        //    timeOutAnswer = 0;
        //    AskLampInfoStatus(address1, address2);
        //    redValueOld = ReceivedAnswer.answer[1];
        //    greenValueOld = ReceivedAnswer.answer[2];
        //    blueValueOld = ReceivedAnswer.answer[3];
        //    brightnessValueOld = ReceivedAnswer.answer[4];

        //    return CheckAnswer();
        //}

        /**
          * description  It sets PL1167's SYNCWORD0 of bulb
          * param        address1      : high address of bulb [0..255]
          * param        address2      : low address of bulb [0..255]
          * param        ui8MSByteSyncWord: most significant byte of SYNCWORD0 [0..255]
          * param        ui8LSByteSyncWord: least significant byte of SYNCWORD0 [0..255]
          * retval       none
          * note         The bulb sends an answer for reporting command results
          * note         This command sets also PL1167's SYNCWORD0 of SHIELD LYT-WIFI card, but it is not saved it in Arduino's EEPROM 
          */
        void SetSyncWord(byte address1, byte address2, byte ui8MSByteSyncWord, byte ui8LSByteSyncWord)
        {
            Command commandToSend = new Command();

            commandToSend.commandType = CommandType.SetSyncWord;
            commandToSend.destinationAddress1 = upperAddressByte;
            commandToSend.destinationAddress2 = lowerAddressByte;
            commandToSend.frameNumber = frameCounter++;
            commandToSend.param1 = ui8MSByteSyncWord;
            commandToSend.param2 = ui8LSByteSyncWord;
            commandToSend.param3 = 0;
            commandToSend.param4 = 0;

            commandToSend.CalculateCRC();

            if (OnCommandReady != null)
            {
                OnCommandReady(this, new CommandReadyEventArgs(commandToSend));
            }
        }

        /**
         * description  It sets PL1167's radio channel of bulb
         * param        address1      : high address of bulb [0..255]
         * param        address2      : low address of bulb [0..255]
         * param        ui8Channel       : radio channel of bulb [0..127]
         * retval       none
         * note         The bulb sends an answer for reporting command results
         * note         This command sets also PL1167's radio channel of SHIELD LYT-WIFI card, but it not saves it in Arduino's EEPROM
         */
        void SetChannel(byte channel)
        {
            if (channel < 128)
            {
                Command commandToSend = new Command();

                commandToSend.commandType = CommandType.SetChannel;
                commandToSend.destinationAddress1 = upperAddressByte;
                commandToSend.destinationAddress2 = lowerAddressByte;
                commandToSend.frameNumber = 0;
                commandToSend.param1 = channel;
                commandToSend.param2 = 0;
                commandToSend.param3 = 0;
                commandToSend.param4 = 0;

                commandToSend.CalculateCRC();

                if (OnCommandReady != null)
                {
                    OnCommandReady(this, new CommandReadyEventArgs(commandToSend));
                }
            }
            else
            {
                throw new Exception("ERROR. Channel too high. Select a channel from 0 to 127.");
            }
        }

        #endregion
    }
}

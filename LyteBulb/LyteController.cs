using SmartLights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using System.Collections;
using Radio.PL1167;

namespace SmartLights.Lyte
{
    public class LyteController : ISmartLightController
    {
        //private bool setLocalSyncWordPending = false;
        //private bool setLocalChannelPending = false;
        //private bool answerAvailable = false;
        //private byte timeOutAnswer;
        //private ushort timeOutCounter;
        //private byte sentCommandCounter;
        //private byte frameCounter = 0;
        //private ProtocolStatus LampProtocolStatus = ProtocolStatus.Init;
        //private byte ui8MSByteSyncWord0, ui8LSByteSyncWord0, ui8Channel0;
        //private byte repetitionTimes = Constants.PROTOCOL_COMMAND_REPETITION;

        //public byte ui8SentCommandCounterMem; // Used for radio channel test
        //public CommandStatus CommandStatus = CommandStatus.Idle;
        //public Address_Table_t LampAddressTable;

        //public Command commandToSend;
        //public Answer ReceivedAnswer;
        private PL1167 radio; 

        private Dictionary<short, Lyte> lights = new Dictionary<short, Lyte>();

        public int Count
        {
            get
            {
                return lights.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public ICollection<object> Keys
        {
            get
            {
                return (ICollection<object>)lights.Keys.Cast<object>();
            }
        }

        public ICollection<ISmartLight> Values
        {
            get
            {
                return (ICollection<ISmartLight>)lights.Keys.Cast<ISmartLight>();
            }
        }

        public ISmartLight this[object key]
        {
            get
            {
                return lights[(short)key];
            }
            set
            {
                ((Lyte)value).OnCommandReady += Lyte_OnCommandReady;
                lights[(short)key] = (Lyte)value;
            }
        }

        public LyteController(PL1167 radio)
        {
            this.radio = radio;
        }

        private void Lyte_OnCommandReady(object sender, CommandReadyEventArgs e)
        {
            //SendCommand....
            radio.WriteTXFIFO(Converters.StructureToBytes(e.CommandToSend));
        }

        /**
          * description  Protocol management
          * param        none
          * retval       none
          */
//        void ProtocolTask()
//        {
//            switch (LampProtocolStatus)
//            {
//                case ProtocolStatus.Init:
//                    vfSetLocalChannel(EEPROMValues.ui8Channel0);
//                    vfSetLocalSyncWord(EEPROMValues.ui8MSByteSyncWord0, EEPROMValues.ui8LSByteSyncWord0);
//                    LampProtocolStatus = ProtocolStatus.Idle;
//                    break;

//                case ProtocolStatus.Idle:
//                    if (CommandStatus == CommandStatus.ToBeSend)
//                    {
//                        ui8SentCommandCounter = 0;
//                        LampProtocolStatus = ProtocolStatus.SendCommand;
//                    }
//                    break;

//                case ProtocolStatus.SendCommand:
//                    ui16TimeOutCounter = 0;
//                    ui8SentCommandCounter++;
//                    if (ui8SentCommandCounter <= Constants.PROTOCOL_SEND_TIMES)
//                    {
//                        CommandStatus = CommandStatus.Send;
//                        LampProtocolStatus = ProtocolStatus.RECEIVE_ACK_AND_ANSWER;

//                        WriteTXFIFO(Convert.StructureToBytes(commandToSend));

//                        if (bSetLocalSyncWordPending)
//                        {
//                            bSetLocalSyncWordPending = false;
//                            vfSetLocalSyncWord(ui8MSByteSyncWord0, ui8LSByteSyncWord0);
//                        }
//                        if (bSetLocalChannelPending)
//                        {
//                            bSetLocalChannelPending = false;
//                            vfSetLocalChannel(ui8Channel0);
//                        }
//#if defined(__DEBUG__)
//				    vfSendCommandToSerial();
//#endif
//                    }
//                    else
//                    {
//                        CommandStatus = CommandStatus.Complete;
//                        LampProtocolStatus = ProtocolStatus.Idle;
//                        vfSetAnswerFail(Constants.C_ERROR_NO_ANSWER);
//                    }
//                    break;

//                case ProtocolStatus.ReceiveAckAndAnswer:
//                    ui16TimeOutCounter++;
//                    if (bRXBufferAvailable)
//                    {
//                        answerAvailable = true;
//                        bRXBufferAvailable = false;
//                        CommandStatus = CommandStatus.Complete;
//                        LampProtocolStatus = ProtocolStatus.Idle;

//                        if (!ui16fReadStatusReg(Constants.C_CRC_ERR))
//                        {
//                            if (ui8RXBufferLength <= sizeof(Answer_Struct_t))
//                            {
//                                memcpy(ReceivedAnswer.AnswerString, sRXBuffer, ui8RXBufferLength);
//                                if (bfCRCCheck())
//                                {
//#if defined(__DEBUG__)
//                  vfSendAnswerToSerial();
//#endif
//                                    if (bfVerifyAnswer())
//                                    {
//                                        ui8SentCommandCounterMem = ui8SentCommandCounter; // Used for radio channel test
//                                        if (ReceivedAnswer.answerTocommandType == CommandType.ReadAddressTable)
//                                            memcpy(LampAddressTable.ui8AddressVector, ReceivedAnswer.ui8Answer, PROTOCOL_ANSWER_LENGHT);
//                                    }
//                                    else
//                                        vfSetAnswerFail(Constants.C_ERROR_WRONG_ANSWER);
//                                }
//                                else
//                                    vfSetAnswerFail(Constants.C_ERROR_WRONG_ANSWER);
//                            }
//                            else
//                                vfSetAnswerFail(Constants.C_ERROR_WRONG_ANSWER);
//                        }
//                        else
//                            vfSetAnswerFail(Constants.C_ERROR_BAD_ANSWER_CRC);
//                    }
//                    else
//                    {
//                        if (!(commandToSend.commandType & Constants.C_MULTICAST))
//                        {
//                            if (ui16TimeOutCounter > ((1000 * PROTOCOL_TIMEOUT) / PROTOCOL_TIME_PERIOD))
//                            {
//                                ui8SentCommandCounterMem = ui8SentCommandCounter; // Used for radio channel test
//                                CommandStatus = COMMAND_TO_BE_SENT;
//                                LampProtocolStatus = PROTOCOL_SEND_COMMAND;
//                            }
//                        }
//                        else
//                        {
//                            CommandStatus = CommandStatus.Complete;
//                            LampProtocolStatus = ProtocolStatus.Idle;
//                        }
//                    }
//                    break;

//                default:
//                    LampProtocolStatus = ProtocolStatus.Idle;
//                    break;
//            }
//            if (CommandStatus == CommandStatus.Complete)
//                CommandStatus = CommandStatus.Idle;
//        }

//        /**
//          * description  It sets command fields
//          * param        none
//          * retval       none
//          */
//        byte SendCommand()
//        {
//            if ((CommandStatus == CommandStatus.Idle) || (CommandStatus == CommandStatus.Complete))
//            {
//                bAnswerAvailable = false;
//                bRXBufferAvailable = false;
//                ReceivedAnswer.answerTocommandType = NO_COMMAND_SEND;
//                ReceivedAnswer.sourceAddress1 = Constants.C_INVALID_ADDRESS;
//                ReceivedAnswer.sourceAddress2 = Constants.C_INVALID_ADDRESS;
//                ReceivedAnswer.answerType = Constants.C_ACK;
//                ReceivedAnswer.answerToFrameNumber = 0;
//                memset(ReceivedAnswer.answer, 0, PROTOCOL_ANSWER_LENGHT);
//                CommandStatus = CommandStatus.ToBeSend;
//                frameCounter++;
//                if (frameCounter > 255) // redundant control
//                    frameCounter = 0;
//                commandToSend.frameNumber = frameCounter;
//                commandToSend.crc ^= commandToSend.frameNumber;

//                return Constants.C_COMMAND_ACK;
//            }
//            else
//                return Constants.C_COMMAND_NACK;
//        }

//        /**
//          * description  It verifies received answer
//          * param        none
//          * retval       none
//          */
//        bool VerifyAnswer()
//        {
//            return ((commandToSend.commandType == ReceivedAnswer.answerTocommandType) &&
//                    (commandToSend.destinationAddress1 == ReceivedAnswer.sourceAddress1) &&
//                    (commandToSend.destinationAddress2 == ReceivedAnswer.sourceAddress2) &&
//                  (commandToSend.frameNumber == ReceivedAnswer.answerToFrameNumber));
//        }

//        /**
//          * description  It sets fields of answer, if an error is received
//          * param        ui8TypeError: error type
//          * retval       none
//          */
//        void SetAnswerFail(byte ui8TypeError)
//        {
//            ReceivedAnswer.answerTocommandType = commandToSend.commandType;
//            ReceivedAnswer.sourceAddress1 = commandToSend.destinationAddress1;
//            ReceivedAnswer.sourceAddress2 = commandToSend.destinationAddress2;
//            ReceivedAnswer.answerType = ui8TypeError;
//            ReceivedAnswer.answerToFrameNumber = commandToSend.frameNumber;
//            memset(ReceivedAnswer.answer, '\0', PROTOCOL_ANSWER_LENGHT);
//        }

//        /**
//          * description  
//          * param        none
//          * retval       none
//          */
//        byte CheckAnswer()
//        {
//            while ((LampProtocolStatus != ProtocolStatus.Idle) || ((CommandStatus != CommandStatus.Idle) && (CommandStatus != CommandStatus.Complete)))
//                delay(PROTOCOL_ANSWER_TIME_PERIOD);
//            while ((!bAnswerAvailable) && (timeOutAnswer <= ((1000 * Constants.PROTOCOL_ANSWER_TIMEOUT) / Constants.PROTOCOL_ANSWER_TIME_PERIOD)))
//            {
//                answerAvailable = false;
//                timeOutAnswer++;
//                delay(PROTOCOL_ANSWER_TIME_PERIOD);
//            }
//            if (ReceivedAnswer.answerTocommandType != CommandType.NoCommandSend)
//                return ReceivedAnswer.answerType;
//            else
//                return Constants.C_ERROR_NO_ANSWER;
//        }

        
        /**
          * description  It sets RGB values of the bulb and it returns answer received from it
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
          * retval       Constants.C_ACK                          0x00  Bulb receives good command
          *              Constants.C_NACK                         0x01  Bulb receives corrupt command
          *              Constants.C_ERROR_COMMAND_IN_PROGRESS    0x06  Bulb's is busy to do a previous command
          *              Constants.C_ERROR_BAD_ANSWER_CRC         0x10  LYT shield receives corrupt answer
          *              Constants.C_ERROR_NO_ANSWER              0x11  LYT shield does not receive answer by Bulb
          *              Constants.C_ERROR_WRONG_ANSWER           0x12  LYT shield receives an answer not corresponding to sent command
          * note         In Constants.C_MULTICAST mode, retval is always Constants.C_ACK
          */
        //byte SetRGBValuesAndCheck(byte address1, byte address2, byte redValue, byte greenValue, byte blueValue, byte castType, byte dimmerSteps)
        //{
        //    byte ui8Counter, step;

        //    step = 0;
        //    timeOutAnswer = 0;
        //    if (castType == Constants.C_UNICAST)
        //    {
        //        SetRGBValues(address1, address2, redValue, greenValue, blueValue, Constants.C_UNICAST, dimmerSteps);

        //        return CheckAnswer();
        //    }
        //    else
        //    {
        //        for (ui8Counter = 0; ui8Counter < repetitionTimes; ui8Counter++)
        //        {
        //            if ((ui8Counter % 3) == 0)
        //                step++;
        //            SetRGBValues(address1, address2, (byte)(redValueOld + (byte)((((float)(redValue - redValueOld)) / repetitionTimes) * 3 * step)),
        //                                                     (byte)(greenValueOld + (byte)((((float)(greenValue - greenValueOld)) / repetitionTimes) * 3 * step)),
        //                                                     (byte)(blueValueOld + (byte)((((float)(blueValue - blueValueOld)) / repetitionTimes) * 3 * step)), 
        //                                                     Constants.C_MULTICAST, 0);
        //            delayMicroseconds(300);
        //        }
        //        redValueOld = redValue;
        //        greenValueOld = greenValue;
        //        blueValueOld = blueValue;
        //        brightnessValueOld = 0;

        //        return Constants.C_ACK;
        //    }
        //}

       
        /**
          * description  It sets Brightness values of the bulb and it returns answer received from it
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
          * retval       Constants.C_ACK                          0x00  Bulb receives good command
          *              Constants.C_NACK                         0x01  Bulb receives corrupt command
          *              Constants.C_ERROR_COMMAND_IN_PROGRESS    0x06  Bulb's is busy to do a previous command
          *              Constants.C_ERROR_BAD_ANSWER_CRC         0x10  LYT shield receives corrupt answer
          *              Constants.C_ERROR_NO_ANSWER              0x11  LYT shield does not receive answer by Bulb
          *              Constants.C_ERROR_WRONG_ANSWER           0x12  LYT shield receives an answer not corresponding to sent command
          * note         In Constants.C_MULTICAST mode, retval is always Constants.C_ACK
          */
        //byte SetBrightnessValueAndCheck(byte address1, byte address2, byte brightnessValue, byte castType, byte dimmerSteps)
        //{
        //    byte ui8Counter, step;

        //    step = 0;
        //    timeOutAnswer = 0;
        //    if (castType == Constants.C_UNICAST)
        //    {
        //        SetBrightnessValue(address1, address2, brightnessValue, Constants.C_UNICAST, dimmerSteps);

        //        return CheckAnswer();
        //    }
        //    else
        //    {
        //        //    for(ui8Counter=0; ui8Counter<PROTOCOL_COMMAND_REPETITION; ui8Counter++)
        //        for (ui8Counter = 0; ui8Counter < repetitionTimes; ui8Counter++)
        //        {
        //            //      vfSetBrightnessValue(address1, address2, ui8BrightnessValue, Constants.C_MULTICAST, 0);
        //            if ((ui8Counter % 3) == 0)
        //                step++;
        //            SetBrightnessValue(address1, address2, brightnessValueOld + (byte)(((float)(brightnessValue - brightnessValueOld)) / repetitionTimes * 3 * ui8Step), Constants.C_MULTICAST, 0);
        //            delayMicroseconds(300);
        //        }
        //        redValueOld = 0;
        //        greenValueOld = 0;
        //        blueValueOld = 0;
        //        brightnessValueOld = brightnessValue;

        //        return Constants.C_ACK;
        //    }
        //}

        

        /**
          * description  It sets PL1167's SYNCWORD0 of bulb
          * param        address1      : high address of bulb [0..255]
          * param        address2      : low address of bulb [0..255]
          * param        ui8MSByteSyncWord: most significant byte of SYNCWORD0 [0..255]
          * param        ui8LSByteSyncWord: least significant byte of SYNCWORD0 [0..255]
          * retval       Constants.C_ACK                          0x00  Bulb receives good command
          *              Constants.C_NACK                         0x01  Bulb receives corrupt command
          *              Constants.C_ERROR_COMMAND_IN_PROGRESS    0x06  Bulb's is busy to do a previous command
          *              Constants.C_ERROR_BAD_ANSWER_CRC         0x10  LYT shield receives corrupt answer
          *              Constants.C_ERROR_NO_ANSWER              0x11  LYT shield does not receive answer by Bulb
          *              Constants.C_ERROR_WRONG_ANSWER           0x12  LYT shield receives an answer not corresponding to sent command
          * note         The bulb sends an answer for reporting command results
          * note         This command sets also PL1167's SYNCWORD0 of SHIELD LYT-WIFI card, but it not saves it in Arduino's EEPROM
          */
        //byte SetSyncWordAndCheck(byte address1, byte address2, byte ui8MSByteSyncWord, byte ui8LSByteSyncWord)
        //{
        //    timeOutAnswer = 0;
        //    SetSyncWord(address1, address2, ui8MSByteSyncWord, ui8LSByteSyncWord);

        //    return CheckAnswer();
        //}

        /**
          * description  It sets PL1167's SYNCWORD0 of SHIELD LYT-WIFI card and it saves it in Arduino's EEPROM
          * param        ui8MSByteSyncWord: most significant byte of SYNCWORD0 [0..255]
          * param        ui8LSByteSyncWord: least significant byte of SYNCWORD0 [0..255]
          * param        bSaveToEEPROM    : mode of setting
          *                                 This parameter can be one of following values:
          *                                   false: temporary only (default)
          *                                   true : save the SYNCWORD0 value in EEPROM
          */
        //void SetLocalSyncWord(byte ui8MSByteSyncWord0, byte ui8LSByteSyncWord0, bool bSaveToEEPROM)
        //{
        //    SetSyncWords((((uint16_t)ui8MSByteSyncWord0) << 8) + ui8LSByteSyncWord0);
        //    Serial.print(F("MSByte SyncWord: "));
        //    Serial.println(ui8MSByteSyncWord0);
        //    Serial.print(F("LSByte SyncWord: "));
        //    Serial.println(ui8LSByteSyncWord0);
        //    Serial.println(F(""));
        //    //  if(bSaveToEEPROM)
        //    if (bSaveToEEPROM && (!bSoulissCompatibility))
        //    {
        //        EEPROMValues.ui8MSByteSyncWord0 = ui8MSByteSyncWord0;
        //        EEPROMValues.ui8LSByteSyncWord0 = ui8LSByteSyncWord0;
        //        vfWriteEEPROM();
        //    }
        //}

       

        /**
          * description  It sets PL1167's radio channel of bulb and it returns answer received from it
          * param        address1      : high address of bulb [0..255]
          * param        address2      : low address of bulb [0..255]
          * param        ui8Channel       : radio channel of bulb [0..127]
          * retval       Constants.C_ACK                          0x00  Bulb receives good command
          *              Constants.C_NACK                         0x01  Bulb receives corrupt command
          *              Constants.C_ERROR_COMMAND_IN_PROGRESS    0x06  Bulb's is busy to do a previous command
          *              Constants.C_ERROR_BAD_ANSWER_CRC         0x10  LYT shield receives corrupt answer
          *              Constants.C_ERROR_NO_ANSWER              0x11  LYT shield does not receive answer by Bulb
          *              Constants.C_ERROR_WRONG_ANSWER           0x12  LYT shield receives an answer not corresponding to sent command
          * note         The bulb sends an answer for reporting command results
          * note         This command sets also PL1167's radio channel of SHIELD LYT-WIFI card, but it not saves it in Arduino's EEPROM
          */
        //byte SetChannelAndCheck(byte address1, byte address2, byte ui8Channel)
        //{
        //    timeOutAnswer = 0;
        //    SetChannel(address1, address2, ui8Channel);

        //    return CheckAnswer();
        //}

        /**
          * description  It sets PL1167's radio channel of SHIELD LYT-WIFI card and it saves it in Arduino's EEPROM
          * param        ui8Channel       : radio channel of bulb [0..127]
          * param        bSaveToEEPROM    : mode of setting
          *                                 This parameter can be one of following values:
          *                                   false: temporary only (default)
          *                                   true : save the channel value in EEPROM
          * retval       None
          */
        //void SetLocalChannel(byte ui8Channel0, bool bSaveToEEPROM)
        //{
        //    if (SetRadioTransmission(ui8Channel0))
        //    {
        //        Serial.print(F("Radio channel: "));
        //        Serial.println(ui8Channel0);
        //        //	  if(bSaveToEEPROM)
        //        if (bSaveToEEPROM && (!bSoulissCompatibility))
        //        {
        //            EEPROMValues.ui8Channel0 = ui8Channel0;
        //            vfWriteEEPROM();
        //        }
        //    }
        //    else
        //    {
        //        Serial.print(F("ERROR. Channel too high. Select a channel from 0 to 127."));
        //        Serial.println(F(""));
        //    }
        //}

        /**
          * description  It sets the number of fast command repetion
          * param        ui8Repetition    : number of repetition
          * retval       None
          */
        //void SetNumberFastCommandRepetition(byte ui8Repetition)
        //{
        //    repetitionTimes = ui8Repetition;
        //}


        /**
          * description  It checks crc field of received answer
          * param        none
          * retval       true or false
          */
        //bool bfCRCCheck()
        //{
        //    byte ui8Counter, crc = 0;

        //    for (ui8Counter = 0; ui8Counter < sizeof(Answer) - 1; ui8Counter++)
        //        crc ^= ReceivedAnswer.AnswerString[ui8Counter];

        //    return (crc == ReceivedAnswer.crc);
        //}

        public bool ContainsKey(object key)
        {
            throw new NotImplementedException();
        }

        public void Add(object key, ISmartLight value)
        {
            if (key is short && value is Lyte)
            {
                ((Lyte)value).OnCommandReady += Lyte_OnCommandReady;
                lights.Add((short)key, (Lyte)value);
            }
            else
            {
                throw new Exception("Key needs to be a short and value needs to be a Lyte for this controller");
            }
        }

        public bool Remove(object key)
        {
            if (key is short)
            {
                return lights.Remove((short)key);
            }
            else
            {
                throw new Exception("Key needs to be a short for this controller");
            }
        }

        public bool TryGetValue(object key, out ISmartLight value)
        {
            if (key is short)
            {
                Lyte lyte;
                bool result = lights.TryGetValue((short)key, out lyte);
                value = (ISmartLight)lyte;
                return result;
            }
            else
            {
                throw new Exception("Key needs to be a short for this controller");
            }
            
        }

        public void Add(KeyValuePair<object, ISmartLight> item)
        {
            if (item.Key is short && item.Value is Lyte)
            {
                ((Lyte)item.Value).OnCommandReady += Lyte_OnCommandReady;
                lights.Add((short)item.Key, (Lyte)item.Value);
            }
            else
            {
                throw new Exception("Key needs to be a short and value a Lyte for this controller");
            }
        }

        public void Clear()
        {
            lights.Clear();
        }

        public bool Contains(KeyValuePair<object, ISmartLight> item)
        {
            if (item.Key is short && item.Value is Lyte)
            {
                return (lights.ContainsKey((short)item.Key) && lights[(short)item.Key] == item.Value);
            }
            else
            {
                return false;
            }
        }

        public void CopyTo(KeyValuePair<object, ISmartLight>[] array, int arrayIndex)
        {
            if (array.All(x => x.Key is short && x.Value is ISmartLight))
            {
                foreach(KeyValuePair<object, ISmartLight> item in array)
                {
                    //lights
                }
            }
            else
            {
                throw new Exception("All KeyValuePairs need to have a short as key and a Lyte as value for this controller");
            }
        }

        public bool Remove(KeyValuePair<object, ISmartLight> item)
        {
            if (item.Key is short && item.Value is Lyte)
            {
                return lights.Remove((short)item.Key);
            }
            else
            {
                throw new Exception("KeyValuePair needs to have a short as key and a Lyte as value for this controller");
            }
        }

        public IEnumerator<KeyValuePair<object, ISmartLight>> GetEnumerator()
        {
            return lights.ToDictionary(k => (object)k.Key, k => (ISmartLight)k.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return lights.GetEnumerator();
        }



//#if defined(__DEBUG__)
///**
//  * description  It prints received answer to serial
//  * param        none
//  * retval       none
//  */
//void vfSendAnswerToSerial()
//{
//	String sBufferOut;
//	volatile byte ui8Counter;

//	Serial.println(F("** Answer received **"));
//	sBufferOut="Command type : "+String(ReceivedAnswer.AnswerTocommandType, 16)+"\n\r";
//	Serial.print(sBufferOut);
//	sBufferOut="Address1     : "+String(ReceivedAnswer.ui8SourceAddress1)+"\n\r";
//	Serial.print(sBufferOut);
//	sBufferOut="Address2     : "+String(ReceivedAnswer.ui8SourceAddress2)+"\n\r";
//	Serial.print(sBufferOut);
//	sBufferOut="ACK/Error    : "+String(ReceivedAnswer.ui8AnswerType, 16)+"\n\r";
//	Serial.print(sBufferOut);
//	sBufferOut="Frame number : "+String(ReceivedAnswer.ui8AnswerToFrameNumber)+"\n\r";
//	Serial.print(sBufferOut);
//	if(ReceivedAnswer.AnswerTocommandType==READ_ADDRESS_TABLE)
//	{
//		Serial.print(F("Address table: "));
//		for(ui8Counter=0; ui8Counter<PROTOCOL_ADDRESS_TABLE_POSITIONS; ui8Counter++)
//		{
//			Serial.print(F("("));
//			Serial.print(ReceivedAnswer.ui8Answer[2*ui8Counter], DEC);
//			Serial.print(F(", "));
//			Serial.print(ReceivedAnswer.ui8Answer[2*ui8Counter+1], DEC);
//			Serial.print(F(") "));
//		}
//	}
//	else
//	{
//		Serial.print(F("Answers      : "));
//		for(ui8Counter=0; ui8Counter<PROTOCOL_ANSWER_LENGHT; ui8Counter++)
//		{
//			Serial.print(ReceivedAnswer.ui8Answer[ui8Counter], DEC);
//			Serial.print(F(" "));
//		}
//	}
//	Serial.println(F("\n"));
//}
//#endif


//#if defined(__DEBUG__)
///**
//  * description  It prints sent command to serial
//  * param        none
//  * retval       none
//  */
//void vfSendCommandToSerial()
//{
//	String sBufferOut;
	
//  if((commandToSend.commandType&Constants.C_MULTICAST)==0)
//  {
//	  Serial.println(F("** Command sent **"));
//	  sBufferOut="Command type : "+String(commandToSend.commandType, 16)+"\n\r";
//  	Serial.print(sBufferOut);
//	  sBufferOut="Address1     : "+String(commandToSend.destinationAddress1)+"\n\r";
//	  Serial.print(sBufferOut);
//	  sBufferOut="Address2     : "+String(commandToSend.destinationAddress2)+"\n\r";
//	  Serial.print(sBufferOut);
//	  sBufferOut="Frame number : "+String(commandToSend.frameNumber)+"\n\r";
//	  Serial.print(sBufferOut);
//	  sBufferOut="Param1       : "+String(commandToSend.param1)+"\n\r";
//	  Serial.print(sBufferOut);
//	  sBufferOut="Param2       : "+String(commandToSend.param2)+"\n\r";
//	  Serial.print(sBufferOut);
//	  sBufferOut="Param3       : "+String(commandToSend.param3)+"\n\n\r";
//	  Serial.print(sBufferOut);
//  }
//}
//#endif


    }
}

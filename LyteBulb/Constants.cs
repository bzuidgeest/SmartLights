using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLights.Lyte
{
    public static class Constants
    {
        public const byte C_COMMAND_NACK = 0x00;
        public const byte C_COMMAND_ACK = 0x01;

        public const byte C_UNICAST = 0x00;
        public const byte C_MULTICAST = 0x80;

        public const byte C_DEFAULT_ADDRESS = 0x00;
        public const byte C_INVALID_ADDRESS = 0xFF;

        public const byte C_ACK = 0x00;  // Bulb receives good command
        public const byte C_NACK = 0x01;  // Bulb receives corrupt command
        public const byte C_ERROR_NO_ADDRESS_AVAILABLE = 0x02;  // Bulb has not available positions to store new address
        public const byte C_ERROR_ADDRESS_EXISTING = 0x03;  // Bulb already has the new address
        public const byte C_ERROR_ADDRESS_NOT_PRESENT = 0x04;  // Bulb's address to be deleted is not present in address table
        public const byte C_ERROR_ADDRESS_NOT_DELETABLE = 0x05;  // Bulb's address to be deleted is not delectable
        public const byte C_ERROR_COMMAND_IN_PROGRESS = 0x06;  // Bulb is busy to do a previous command
        public const byte C_ERROR_BAD_ANSWER_CRC = 0x10;  // LYT shield receives corrupt answer
        public const byte C_ERROR_NO_ANSWER = 0x11;  // LYT shield does not receive answer by slave 
        public const byte C_ERROR_WRONG_ANSWER = 0x12;  // LYT shield receives an answer not corresponding to sent command


        public const int PROTOCOL_ADDRESS_TABLE_POSITIONS= 5;
        public const int PROTOCOL_ANSWER_LENGHT   = 2 * PROTOCOL_ADDRESS_TABLE_POSITIONS;
        public const int PROTOCOL_SEND_TIMES      = 15;
        public const int PROTOCOL_TIME_PERIOD     = 10;                  // ms
        public const double PROTOCOL_TIMEOUT            = 0.3;                  // s
        public const int PROTOCOL_ANSWER_TIME_PERIOD    = 100;                   // ms
        public const double PROTOCOL_ANSWER_TIMEOUT        = PROTOCOL_TIMEOUT + 0.5;     // s
        public const int PROTOCOL_DIMMER_STEPS         = 100;
        public const int PROTOCOL_COMMAND_REPETITION   = 30;
    }
}

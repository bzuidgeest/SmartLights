using Radio.PL1167;
using SmartLights.Lyte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LyteTest
{
    class Program
    {
        static void Main(string[] args)
        {
            BPPL1167 radio = new BPPL1167();

            radio.Initialize("COM3");

            PL1167Status currentState = radio.ReadStatusReg();

            bool set = radio.SetRadioChannel(33);
            int channel = radio.GetRadioChannel();

            int syncword0 = radio.GetSyncWord0();

            LyteController controller = new LyteController(radio);
            Lyte lyte = new Lyte(0, TransmissionMode.Multicast);

            controller.Add((short)0, lyte);

            while(true)
            {
                lyte.TurnOff();
                Thread.Sleep(1000);
                lyte.TurnOn();

                if (Console.KeyAvailable)
                    break;
            }
         }
    }
}

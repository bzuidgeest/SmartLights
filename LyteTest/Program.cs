using NAudio.Wave;
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
        static WasapiLoopbackCapture waveIn;

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

            lyte.TurnOn();

            //waveIn = new WasapiLoopbackCapture();
            //waveIn.DataAvailable += InputBufferToFileCallback;
            //waveIn.StartRecording();

            while (true)
            {
                //lyte.TurnOff();
                lyte.SetRGB(new SmartLights.SmartLightColor() { blue = 0, green = 0, red = 255 });
                //lyte.SetWhite(new SmartLights.SmartLightColor() { white = 50 });
                Thread.Sleep(1000);
                //lyte.TurnOn();
                lyte.SetRGB(new SmartLights.SmartLightColor() { blue = 0, green = 255, red = 0 });
                //lyte.SetWhite(new SmartLights.SmartLightColor() { white = 180 });
                Thread.Sleep(1000);

                if (Console.KeyAvailable)
                    break;
            }
         }

        public static void InputBufferToFileCallback(object sender, WaveInEventArgs e)
        {
            // The recorder bytes can be found in e.Buffer
            // The number of bytes recorded can be found in e.BytesRecorded
            // Process the audio data any way you wish...
        }
    }
}

using NAudio.CoreAudioApi;
using Radio.PL1167;
using SmartLights.Lyte;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleSoundMeter
{
    public partial class Form1 : Form
    {
        Lyte lyte;

        public Form1()
        {
            InitializeComponent();

            MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();
            var devices = deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            comboBox1.Items.AddRange(devices.ToArray());

            BPPL1167 radio = new BPPL1167();

            radio.Initialize("COM3");

            PL1167Status currentState = radio.ReadStatusReg();

            bool set = radio.SetRadioChannel(33);
            int channel = radio.GetRadioChannel();

            int syncword0 = radio.GetSyncWord0();

            LyteController controller = new LyteController(radio);
            lyte = new Lyte(0, TransmissionMode.Multicast);

            controller.Add((short)0, lyte);

            lyte.TurnOn();

            timer1.Interval= 10;
            timer1.Enabled = true;

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                var device = (MMDevice)comboBox1.SelectedItem;
                int soundValue = (int)Math.Round(device.AudioMeterInformation.MasterPeakValue * 100);

                //if (soundValue < 30)
                //    lyte.SetRGB(new SmartLights.SmartLightColor() { blue = 0, green =  255, red = 0 });

                //if (soundValue < 70 && soundValue > 40)
                //    lyte.SetRGB(new SmartLights.SmartLightColor() { blue = 0, green = 0xFC, red = 0xFF });

                //if (soundValue > 70)
                //    lyte.SetRGB(new SmartLights.SmartLightColor() { blue = 0, green = 0, red = 255 });

                //lyte.SetRGB(new SmartLights.SmartLightColor() { blue = 0, green = (byte)(255 - soundValue), red = (byte)soundValue});
                lyte.SetRGB(new SmartLights.SmartLightColor() { blue = 0, green = (byte)((255 * (100 - soundValue)) / 100), red = (byte)((255 * soundValue) / 100) });
            }
        }
    }
}
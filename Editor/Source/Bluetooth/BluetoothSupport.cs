using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InTheHand.Net.Bluetooth;

namespace Editor.Bluetooth
{
    class BluetoothSupport
    {

        Guid service;
        public Guid Service
        {
            get { return service; }
        }

        BluetoothRadio br;
        RadioMode radioMode;
        string localAddress;
        string mode;
        string name;

        bool isBluetoothSupported;
        public bool IsBluetoothSupported
        {
            get { return isBluetoothSupported; }
        }

        public BluetoothSupport()
        {
            service = BluetoothService.SerialPort;
            br = BluetoothRadio.PrimaryRadio;
            this.localAddress = br.LocalAddress.ToString();
            this.mode = br.Mode.ToString();
            this.name = br.Name;

            SetSettings();
        }

        private void SetSettings()
        {
            this.isBluetoothSupported = true;
            this.radioMode = br.Mode;
        }

        public void ChangeDeviceMode(string mode)
        {
            switch (mode)
            {
                case "Discoverable":
                    this.br.Mode = RadioMode.Discoverable;
                    break;
                case "PowerOff":
                    this.br.Mode = RadioMode.PowerOff;
                    break;
                default:
                    this.br.Mode = RadioMode.Connectable;
                    break;
            }
        }

        public void MakeDiscoverable()
        {
            if (br.Mode != RadioMode.Discoverable)
                br.Mode = RadioMode.Discoverable;
        }


    }
}

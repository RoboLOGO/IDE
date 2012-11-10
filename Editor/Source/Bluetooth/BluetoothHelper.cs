using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InTheHand.Net;
using InTheHand.Net.Sockets;
using InTheHand.Net.Bluetooth;
using Editor.Bluetooth;
using System.IO;

namespace Editor
{
    class BluetoothHelper
    {
        static BluetoothHelper self;
        BluetoothClient bluetoothClient;
        BluetoothSupport bluetoothSupport;
        BluetoothDeviceInfo[] bluetoothDeviceInfo;

        private BluetoothHelper()
        {
            this.bluetoothClient = new BluetoothClient();
            this.bluetoothSupport = new BluetoothSupport();
        }

        public static BluetoothHelper GetBluetoothHelper()
        {
            if (self == null)
                self = new BluetoothHelper();
            return self;
        }

        public BluetoothDeviceInfo[] Search()
        {
            bluetoothSupport.ChangeDeviceMode("Discoverable");
            bluetoothDeviceInfo = bluetoothClient.DiscoverDevices(5);
            return bluetoothDeviceInfo;
        }

        public void Connect(BluetoothDeviceInfo btDevice)
        {
            bluetoothSupport.ChangeDeviceMode("Connectable");
            BluetoothAddress btAddress = BluetoothAddress.Parse(btDevice.DeviceAddress.ToString());

            if (btDevice.Authenticated || BluetoothSecurity.PairRequest(btAddress, "0000"))
            {
                bluetoothClient.Connect(new BluetoothEndPoint(btAddress, BluetoothService.GenericFileTransfer));
            }
            
        }

        public void Send(string text)
        {
            Stream peerStream = bluetoothClient.GetStream();
            Byte[] buffer = Encoding.ASCII.GetBytes(text);
            peerStream.Write(buffer, 0, buffer.Length);
        }

    }
}

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
            bluetoothDeviceInfo = bluetoothClient.DiscoverDevices();
            return bluetoothDeviceInfo;
        }

        public void Connect(BluetoothDeviceInfo btDevice)
        {
            bluetoothSupport.ChangeDeviceMode("Connectable");
            BluetoothAddress btAddress = BluetoothAddress.Parse(btDevice.DeviceAddress.ToString());
            if (BluetoothSecurity.PairRequest(btAddress, "1234"))
            {
                bluetoothClient.Connect(new BluetoothEndPoint(btAddress, bluetoothSupport.Service));
            }            
        }

        public void Send(string text)
        {
            if (bluetoothClient != null && bluetoothClient.Connected)
            {
                Stream sendStream = bluetoothClient.GetStream();
                Byte[] buffer = Encoding.ASCII.GetBytes(text);
                sendStream.Write(buffer, 0, buffer.Length);
            }
        }

        public string Read()
        {
            string s = "";
            if (bluetoothClient != null && bluetoothClient.Connected)
            {
                Stream readStream = bluetoothClient.GetStream();
                Byte[] buffer = new Byte[1000];
                readStream.Read(buffer, 0, buffer.Length);
                s = Encoding.ASCII.GetString(buffer).Replace("\0", "").Replace("\r\n", "");
            }
            return s;
        }

        public bool IsConnected()
        {
            return bluetoothClient.Connected;
        }

        //~BluetoothHelper()
        //{
        //    if (bluetoothClient != null && bluetoothClient.Connected)                
        //        bluetoothClient.Close();
        //}

    }
}

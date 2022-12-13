using AirStack.Client.Model;
using System;
using System.IO.Ports;

namespace AirStack.Client.Services
{
    public class COMService : IDisposable
    {
        SerialPort _port;
        public void Open(COMSettings settings)
        {
            if (_port.IsOpen)
                _port.Close();

            _port = new SerialPort(settings.COM, settings.BaudRate, settings.Parity, settings.DataBits);
            _port.Handshake = settings.Handshake;

            _port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            _port.Open();
        }

        void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var sp = (SerialPort)sender;
            string data = sp.ReadExisting();
        }

        public void Close()
        {
            _port.Close();
        }

        public void Dispose()
        {
            Close();
        }
    }
}

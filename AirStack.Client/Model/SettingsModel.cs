using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirStack.Client.Model
{
    public class SettingsModel
    {
        public string ServerAdress { get; set; } = "127.0.0.1:8080/AirStack";
        public bool AppOnTop { get; set; } = false;

        public string DataSeparator { get; set; } = string.Empty;
        public COMSettings SerialPortSettings { get; set; } = new();
        public WindowState WindowState { get; set; } = new() { ShouldRestore = false };
    }

    public class COMSettings
    {
        public string COM { get; set; } = "COM3";
        public int BaudRate { get; set; } = 9600;
        public int DataBits { get; set; } = 8;
        public Parity Parity { get; set; } = Parity.None;
        public Handshake Handshake { get; set; } = Handshake.None;
    }

    public class WindowState
    {
        public double Top { get; set; }
        public double Left { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public bool Maximalized { get; set; }

        public bool ShouldRestore { get; set; }
    }

}

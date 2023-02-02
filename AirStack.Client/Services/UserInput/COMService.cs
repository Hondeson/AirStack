using AirStack.Client.Services.Notification;
using AirStack.Client.Services.Settings;
using Serilog;
using System;
using System.IO.Ports;
using System.Threading.Tasks;

namespace AirStack.Client.Services.UserInput
{
    public class COMService : IUserInputProvider
    {
        const string c_Ping = "ping";
        const string c_Pong = "pong";

        public event EventHandler<UserInputDataReceivedArgs> UserInputDataReceived;
        void OnUserInputDataReceived(string data) => UserInputDataReceived?.Invoke(this, new UserInputDataReceivedArgs(data));

        readonly ISettingsProvider _settings;
        readonly INotificationProvider _notify;
        readonly ILogger _log;
        public COMService(ISettingsProvider settings, INotificationProvider notification, ILogger log)
        {
            _log = log;
            _settings = settings;
            _settings.SettingsChanged += settings_SettingsChanged;

            _notify = notification;
        }

        void settings_SettingsChanged(object sender, EventArgs e)
        {
            Close();
            Open();
        }

        SerialPort _port = new();
        public void Open()
        {
            Task.Run(DoOpen);
        }

        async void DoOpen()
        {
            while (_port == null || !_port.IsOpen)
            {
                try
                {
                    if (_port.IsOpen)
                        Close();

                    var comSettings = _settings.Settings.SerialPortSettings;

                    _port = new SerialPort(comSettings.COM, comSettings.BaudRate, comSettings.Parity, comSettings.DataBits);
                    _port.Handshake = comSettings.Handshake;

                    _port.DataReceived += port_DataReceived;
                    _port.Open();

                    return;
                }
                catch (UnauthorizedAccessException e)
                {
                    _log.Error(e.Message);
                    _notify.NotifyError($"{_settings.Settings.SerialPortSettings.COM} se nezdařilo otevřit");
                }
                catch (Exception e)
                {
                    _log.Error(e.Message);
                    _notify.NotifyError($"Neočekávaná chyba!");
                }

                await Task.Delay(5000);
            }
        }

        void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var sp = (SerialPort)sender;
            string data = sp.ReadExisting();

            try
            {
                if (data == c_Ping)
                {
                    sp.WriteLine(c_Pong);
                    return;
                }

                OnUserInputDataReceived(data);
            }
            catch (Exception ex)
            {
                _log.Error("COM port data received event exception: " + ex.Message);
                throw ex;
            }
        }

        public void Close()
        {
            if (!_port.IsOpen)
                return;

            _port.DataReceived -= port_DataReceived;
            _port.Close();
        }

        public void Dispose()
        {
            Close();
        }
    }
}

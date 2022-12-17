using AirStack.Client.Model;
using AirStack.Client.Services.Notification;
using AirStack.Client.ViewModel;
using Newtonsoft.Json;
using Serilog;
using System;
using System.IO;
using System.IO.Ports;

namespace AirStack.Client.Services.Settings
{
    public class SettingsService : ISettingsProvider
    {
        public event EventHandler SettingsChanged;
        void OnSettingsChanged() => SettingsChanged?.Invoke(this, null);

        const string c_SettingFileName = "settings.json";
        string SettingsFilePath { get => Path.Combine(Directory.GetCurrentDirectory(), c_SettingFileName); }

        readonly INotificationProvider _notify;
        readonly ILogger _log;
        public SettingsService(INotificationProvider notify, ILogger log)
        {
            _log = log;
            _notify = notify;
        }

        public SettingsModel Settings { get; set; } = new();

        public void Load()
        {
            if (!File.Exists(SettingsFilePath))
                return;

            try
            {
                var settings = JsonConvert.DeserializeObject<SettingsModel>(File.ReadAllText(SettingsFilePath));
                Settings = settings;
            }
            catch (Exception e)
            {
                _log.Error(e.Message);
                _notify.NotifyError("Chyba při načtení nastavení");
            }
        }

        public void Save(bool notifyChange = false)
        {
            try
            {
                var json = JsonConvert.SerializeObject(Settings);
                File.WriteAllText(SettingsFilePath, json);

                if (notifyChange)
                    OnSettingsChanged();
            }
            catch (Exception e)
            {
                _log.Error(e.Message);
                _notify.NotifyError("Chyba při uložení nastavení");
            }
        }
    }
}

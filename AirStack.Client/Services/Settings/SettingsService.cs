using AirStack.Client.Model;
using AirStack.Client.ViewModel;
using Newtonsoft.Json;
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

        public SettingsModel Settings { get; set; } = new();

        public void Load()
        {
            if (!File.Exists(SettingsFilePath))
                return;

            var settings = JsonConvert.DeserializeObject<SettingsModel>(File.ReadAllText(SettingsFilePath));
            Settings = settings;
        }

        public void Save()
        {
            var json = JsonConvert.SerializeObject(Settings);
            File.WriteAllText(SettingsFilePath, json);

            OnSettingsChanged();
        }
    }
}

using AirStack.Client.Model;
using System;

namespace AirStack.Client.Services.Settings
{
    public interface ISettingsProvider
    {
        event EventHandler SettingsChanged;
        SettingsModel Settings { get; set; }

        void Load();
        void Save(bool notifyChange = false);
    }
}
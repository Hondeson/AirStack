using AirStack.Client.Model;
using AirStack.Client.Services.Settings;
using AirStack.Client.Validation;
using AirStack.Client.ViewModel.Base;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Windows.Input;

namespace AirStack.Client.ViewModel
{
    public class SettingsVM : BaseVM
    {
        readonly ISettingsProvider _settingsSvc;
        public SettingsVM(ISettingsProvider settingsSvc)
        {
            _settingsSvc = settingsSvc;
            SaveCommand = new RelayCommand(OnSaveCommand);

            Load();
        }

        void Load()
        {
            _settingsSvc.Load();

            this.DataSeparator = _settingsSvc.Settings.DataSeparator;
            this.ServerAdress = _settingsSvc.Settings.ServerAdress;
            this.AppOnTop = _settingsSvc.Settings.AppOnTop;

            var com = _settingsSvc.Settings.SerialPortSettings;
            this.COM = com.COM;
            this.BaudRate = com.BaudRate.ToString();
            this.DataBits = com.DataBits.ToString();
            this.Parity = com.Parity;
            this.Handshake = com.Handshake;

            var comList = new string[100];
            for (int i = 1; i <= 100; i++)
                comList[i - 1] = $"COM{i}";

            COMCollection = new(comList);

            var baudList = new List<string>(11);
            for (int i = 600; i <= 115200; i = i * 2)
                baudList.Add(i.ToString());

            BaudRateCollection = new(baudList);
            DataBitsCollection = new() { "7", "8" };
            ParityCollection = new(Enum.GetValues<Parity>());
            HandshakeCollection = new(Enum.GetValues<Handshake>());
        }

        #region Props
        ObservableCollection<string> _COMCollection;
        public ObservableCollection<string> COMCollection
        {
            get => _COMCollection;
            set => Set(ref _COMCollection, value);
        }

        string _COM;
        [ComboBoxRequired]
        public string COM
        {
            get => _COM;
            set => Set(ref _COM, value);
        }

        ObservableCollection<string> _BaudRateCollection;
        public ObservableCollection<string> BaudRateCollection
        {
            get => _BaudRateCollection;
            set => Set(ref _BaudRateCollection, value);
        }

        string _BaudRate;
        [ComboBoxRequired]
        public string BaudRate
        {
            get => _BaudRate;
            set
            {
                Set(ref _BaudRate, value);
                ValidateProperty(this, value);
            }
        }

        ObservableCollection<string> _DataBitsCollection;
        public ObservableCollection<string> DataBitsCollection
        {
            get => _DataBitsCollection;
            set => Set(ref _DataBitsCollection, value);
        }

        string _DataBits;
        [ComboBoxRequired]
        public string DataBits
        {
            get => _DataBits;
            set => Set(ref _DataBits, value);
        }

        ObservableCollection<Parity> _ParityCollection;
        public ObservableCollection<Parity> ParityCollection
        {
            get => _ParityCollection;
            set => Set(ref _ParityCollection, value);
        }

        Parity _Parity;
        [ComboBoxRequired]
        public Parity Parity
        {
            get => _Parity;
            set => Set(ref _Parity, value);
        }

        ObservableCollection<Handshake> _HandshakeCollection;
        public ObservableCollection<Handshake> HandshakeCollection
        {
            get => _HandshakeCollection;
            set => Set(ref _HandshakeCollection, value);
        }

        Handshake _HandShake;
        [ComboBoxRequired]
        public Handshake Handshake
        {
            get => _HandShake;
            set => Set(ref _HandShake, value);
        }

        string _DataSeparator;
        public string DataSeparator
        {
            get => _DataSeparator;
            set
            {
                Set(ref _DataSeparator, value);
                ValidateProperty(this, value);
            }
        }

        string _ServerAdress;
        public string ServerAdress
        {
            get => _ServerAdress;
            set => Set(ref _ServerAdress, value);
        }

        bool _AppOnTop;
        public bool AppOnTop
        {
            get => _AppOnTop;
            set => Set(ref _AppOnTop, value);
        }
        #endregion

        public ICommand SaveCommand { get; }
        void OnSaveCommand()
        {
            if (!ValidateAllProperties(this))
                return;

            var obj = new SettingsModel()
            {
                ServerAdress = this.ServerAdress,
                AppOnTop = this.AppOnTop,

                DataSeparator = this.DataSeparator,
                SerialPortSettings = new()
                {
                    COM = this.COM,
                    DataBits = int.Parse(this.DataBits),
                    Parity = this.Parity,
                    BaudRate = int.Parse(this.BaudRate)
                }
            };

            _settingsSvc.Settings = obj;
            _settingsSvc.Save(true);
        }
    }
}

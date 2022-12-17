using AirStack.Client.Model;
using AirStack.Client.Services.RequestToServer;
using AirStack.Client.Services.Settings;
using AirStack.Client.Services.UserInput;
using AirStack.Client.ViewModel.Base;
using AirStack.Core.Model;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AirStack.Client.ViewModel
{
    public class ScanCodeVM : BaseVM
    {
        readonly IUserInputProvider _userInput;
        readonly ISettingsProvider _settings;
        readonly IServerRequestService _client;
        public ScanCodeVM(IUserInputProvider userInput, ISettingsProvider settings, IServerRequestService client)
        {
            _client = client;
            _settings = settings;
            _userInput = userInput;
            _userInput.UserInputDataReceived += userInput_UserInputDataReceived;
        }

        public override void Initialize()
        {
            _userInput.Open();
            base.Initialize();
        }

        void userInput_UserInputDataReceived(object sender, UserInputDataReceivedArgs e)
        {
            var dataList = new List<RequestResultObject>();

            if (!string.IsNullOrEmpty(_settings.Settings.DataSeparator))
            {
                var data = e.Data.Split(_settings.Settings.DataSeparator, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                foreach (var code in data)
                    dataList.Add(new RequestResultObject(code));
            }
            else
                dataList.Add(new RequestResultObject(e.Data));

            RaiseOnUI(() =>
            {
                ScannedSNCollection = new(dataList);
                MakeServerRequest(dataList);
            });
        }

        readonly object _reqLock = new();
        volatile bool _accesed = false;
        void MakeServerRequest(List<RequestResultObject> codes)
        {
            //pro zobrazení animace musí vykonat UI vlákno
            this.IsBusy = true;
            _accesed = true;

            Task.Run(() =>
            {
                lock (_reqLock)
                {
                    _accesed = false;
                    foreach (var item in codes)
                    {
                        var result = _client.SendRequestAsync(new ItemModel() { Code = item.Code }).Result;
                        item.Copy(result);
                    }

                    this.RequestResult = codes.All(y => y.Result == true);
                    if (!_accesed)
                        this.IsBusy = false;
                }
            });
        }

        bool? _RequestResult;
        public bool? RequestResult
        {
            get => _RequestResult;
            set => Set(ref _RequestResult, value);
        }

        ObservableCollection<RequestResultObject> _ScannedSNCollection = new();
        public ObservableCollection<RequestResultObject> ScannedSNCollection
        {
            get => _ScannedSNCollection;
            set
            {
                Set(ref _ScannedSNCollection, value);
                OnPropertyChanged(nameof(IsMultipleSN));
            }
        }

        public bool IsMultipleSN { get => ScannedSNCollection.Count > 1; }
    }
}

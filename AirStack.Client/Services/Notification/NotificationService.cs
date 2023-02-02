using AirStack.Client.View;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows;

namespace AirStack.Client.Services.Notification
{
    public class DisplayNotificationService : INotificationProvider, IDisposable
    {
        public DisplayNotificationService()
        { }

        public void Notify(string message)
        {
            throw new NotImplementedException();
        }

        ErrorWindow _errorWindow;
        public void NotifyError(string displayMessage)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (_errorWindow == null || !_errorWindow.IsVisible)
                    InitializeNewWindow();

                if (_errorWindow.Owner is null)
                {
                    try
                    {
                        var _owner = Locator.Resolve<MainView>();
                        _errorWindow.Owner = _owner;
                    }
                    catch { }
                }

                _errorWindow.ErrorText = displayMessage;
                _errorWindow.ErrorAnimation = true;

                if (!_errorWindow.IsVisible)
                    _errorWindow.Show();
            }));
        }

        void InitializeNewWindow()
        {
            _errorWindow = new();
            _errorWindow.Topmost = true;
            _errorWindow.OkCommand = new RelayCommand(OnOkCommand);
        }

        void OnOkCommand()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                _errorWindow.Close();
                _errorWindow.ErrorAnimation = false;
            }));
        }

        public void Dispose()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                _errorWindow?.Close();
            }));
        }
    }
}


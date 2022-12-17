using AirStack.Client.Services.Navigation;
using AirStack.Client.Services.Notification;
using AirStack.Client.Services.Settings;
using AirStack.Client.View;
using AirStack.Client.ViewModel.Base;
using AirStack.Core.Base;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AirStack.Client.ViewModel
{
    public class MainVM : BaseVM
    {
        public MainVM(INavigationService nav)
        {
            Navigation = nav;
            Navigation.ViewChanged += Navigation_ViewChanged;

            SettingsCommand = new RelayCommand(OnSettingsCommand);
            GoBackCommand = new RelayCommand(OnGoBackCommand);

            this.Title = "AirStack";
        }

        void Navigation_ViewChanged(object sender, NavigationViewChangedArgs e)
        {
            if (e.NewView.GetType() == typeof(SettingsView))
                CanGoToSettings = false;
            else
                CanGoToSettings = true;
        }

        bool _CanGoToSettings = true;
        public bool CanGoToSettings
        {
            get => _CanGoToSettings;
            set => Set(ref _CanGoToSettings, value);
        }

        public INavigationService Navigation { get; }

        public ICommand SettingsCommand { get; }
        public void OnSettingsCommand()
        {
            Navigation.PushView(Locator.Resolve<SettingsView>());
        }

        public ICommand GoBackCommand { get; }
        public void OnGoBackCommand()
        {
            if (!Navigation.CanGoBack)
                return;

            Navigation.PopView();
        }
    }
}

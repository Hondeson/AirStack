using AirStack.Client.Services.Navigation;
using AirStack.Client.Services.Settings;
using AirStack.Client.View;
using AirStack.Client.ViewModel.Base;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace AirStack.Client.ViewModel
{
    public class MainVM : BaseVM
    {
        readonly ISettingsProvider _settings;
        public MainVM(INavigationService nav, ISettingsProvider settings)
        {
            _settings = settings;
            Navigation = nav;
            Navigation.ViewChanged += Navigation_ViewChanged;

            SettingsCommand = new RelayCommand(OnSettingsCommand);
            GoBackCommand = new RelayCommand(OnGoBackCommand);

            SetTitleByActualMode();
        }

        void SetTitleByActualMode()
        {
            switch (_settings.Settings.AppMode)
            {
                case Core.Model.StatusEnum.Production:
                    Title = "Airstack Produkce";
                    break;
                case Core.Model.StatusEnum.Tests:
                    Title = "Airstack Testy";
                    break;
                case Core.Model.StatusEnum.Dispatched:
                    Title = "Airstack Expedice";
                    break;
                case Core.Model.StatusEnum.Complaint:
                    Title = "Airstack Reklamace";
                    break;
                case Core.Model.StatusEnum.ComplaintToSupplier:
                    Title = "Airstack Reklamace dodavateli";
                    break;
            }
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

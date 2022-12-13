using AirStack.Client.ViewModel.Base;
using System;
using System.Windows.Controls;

namespace AirStack.Client.Services.Navigation
{
    public interface INavigationService
    {
        event EventHandler<NavigationViewChangedArgs> ViewChanged;
        bool CanGoBack { get; }

        UserControl ActualView { get; }
        BaseVM ActualVM { get; }

        void PushView(UserControl view);
        void PopView();
    }
}
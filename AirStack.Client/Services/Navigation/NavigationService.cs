using AirStack.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AirStack.Client.Services.Navigation
{
    internal class NavigationService : ObservableObject, INavigationService
    {
        UserControl _ActualView;
        public UserControl ActualView
        {
            get => _ActualView;
            set => Set(ref _ActualView, value);
        }

        public object ActualVM { get => ActualView?.DataContext; }

        public void PushView(UserControl view)
        {
            this.ActualView = view;
        }
    }
}

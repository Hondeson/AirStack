using AirStack.Client.ViewModel;
using System.Windows.Controls;

namespace AirStack.Client.View
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView(SettingsVM vm)
        {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}

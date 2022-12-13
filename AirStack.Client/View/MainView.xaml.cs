using AirStack.Client.Services.Settings;
using AirStack.Client.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AirStack.Client.View
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        readonly ISettingsProvider _settSvc;
        public MainView(ISettingsProvider settings)
        {
            _settSvc = settings;
            _settSvc.SettingsChanged += settSvc_SettingsChanged;

            this.MouseLeftButtonDown += Window_MouseLeftButtonDown;
            this.Closing += Window_Closing;
            //this.Loaded += MainView_Loaded;

            InitializeComponent();

            RestoreWindow();
        }

        private void settSvc_SettingsChanged(object sender, EventArgs e)
        {
            this.Topmost = _settSvc.Settings.AppOnTop;
        }

        private void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            RestoreWindow();
        }

        void RestoreWindow()
        {
            var w = _settSvc.Settings.WindowState;
            if (!w.ShouldRestore)
                return;

            this.Top = w.Top;
            this.Left = w.Left;

            this.Height = w.Height;
            this.Width = w.Width;

            if (w.Maximalized)
                this.WindowState = WindowState.Maximized;

            this.Topmost = _settSvc.Settings.AppOnTop;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var w = _settSvc.Settings.WindowState;
            w.Top = this.Top;
            w.Left = this.Left;

            w.Width = this.Width;
            w.Height = this.Height;

            w.Maximalized = this.WindowState == WindowState.Maximized;
            w.ShouldRestore = true;

            _settSvc.Save();
        }
    }
}

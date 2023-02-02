using AirStack.Client.Services.Settings;
using System;
using System.Windows;
using System.Windows.Input;

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

            _settSvc.Save(false);
        }
    }
}

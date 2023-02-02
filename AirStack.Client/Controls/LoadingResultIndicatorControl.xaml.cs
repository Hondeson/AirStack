using AirStack.Client.ViewModel.Base;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace AirStack.Client.Controls
{
    /// <summary>
    /// Interaction logic for LoadingResultIndicatorControl.xaml
    /// </summary>
    public partial class LoadingResultIndicatorControl : UserControl
    {
        public LoadingResultIndicatorControl()
        {
            InitializeComponent();
            this.Loaded += LoadingResultIndicatorControl_Loaded;
            this.Unloaded += LoadingResultIndicatorControl_Unloaded;
        }

        private void LoadingResultIndicatorControl_Unloaded(object sender, RoutedEventArgs e)
        {
            var vm = (BaseVM)this.DataContext;
            vm.IsBusyChanged -= Vm_IsBusyChanged;
        }

        private void LoadingResultIndicatorControl_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = (BaseVM)this.DataContext;
            vm.IsBusyChanged += Vm_IsBusyChanged;
        }

        private void Vm_IsBusyChanged(object sender, BusyChangedArgs e)
        {
            OnIsBusyChanged(e.IsBusy, e.Result);
        }

        Timer timer;
        volatile bool _display = false;
        public void OnIsBusyChanged(bool isBusy, bool? result)
        {
            if (isBusy == true)
            {
                if (timer is not null)
                    return;

                timer = new Timer((obj) =>
                {
                    _display = true;
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (!_display)
                            return;

                        grid.Background = Brushes.Transparent;
                        progressBar.Visibility = Visibility.Visible;
                        progressBar.IsIndeterminate = true;
                    }));
                }, null, 300, Timeout.Infinite);
            }
            else
            {
                _display = false;

                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    timer?.Dispose();
                    timer = null;

                    SetGridAnimation(result);
                    SetResultText(result);

                    progressBar.Visibility = Visibility.Collapsed;
                    progressBar.IsIndeterminate = false;
                }));
            }
        }

        private void SetResultText(bool? result)
        {
            if (result == true)
                this.resultText.Text = "OK";
            else if (result == false)
                this.resultText.Text = "NOK";
            else
                this.resultText.Text = "";
        }

        const string c_RedHex = "#f03232";
        SolidColorBrush GetBrushByResult(bool? result)
        {
            if (result == true)
                return new SolidColorBrush(Colors.LightGreen);

            else if (result == false)
                return (SolidColorBrush)new BrushConverter().ConvertFrom(c_RedHex);

            return new SolidColorBrush() { Color = Colors.Transparent };
        }

        void SetGridAnimation(bool? result)
        {
            SolidColorBrush brush = GetBrushByResult(result);
            brush.Opacity = 0.6;

            ColorAnimation animation = new ColorAnimation();
            animation.To = brush.Color;
            animation.From = Colors.Transparent;
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.6));

            grid.Background = brush;
            brush.BeginAnimation(SolidColorBrush.ColorProperty, animation);
        }
    }
}

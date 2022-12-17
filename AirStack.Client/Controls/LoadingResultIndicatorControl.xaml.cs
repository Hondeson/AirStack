using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        }



        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsBusy.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.Register("IsBusy", typeof(bool), typeof(LoadingResultIndicatorControl), new PropertyMetadata(false, IsBusyChanged));

        private static void IsBusyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as LoadingResultIndicatorControl;
            sender?.OnIsBusyChanged();
        }

        Timer timer;
        volatile bool _display = false;
        public void OnIsBusyChanged()
        {
            if (this.IsBusy == true)
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

                        this.Result = null;

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

                    SetGridAnimation();

                    progressBar.Visibility = Visibility.Collapsed;
                    progressBar.IsIndeterminate = false;
                }));
            }
        }

        const string c_RedHex = "#f03232";
        SolidColorBrush GetBrushByResult()
        {
            if (this.Result == true)
                return new SolidColorBrush(Colors.LightGreen);

            else if (this.Result == false)
                return (SolidColorBrush)new BrushConverter().ConvertFrom(c_RedHex);

            return new SolidColorBrush() { Color = Colors.Transparent };
        }

        void SetGridAnimation()
        {
            SolidColorBrush brush = GetBrushByResult();
            brush.Opacity = 0.6;

            ColorAnimation animation = new ColorAnimation();
            animation.To = brush.Color;
            animation.From = Colors.Transparent;
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.6));

            grid.Background = brush;
            brush.BeginAnimation(SolidColorBrush.ColorProperty, animation);
        }

        public bool? Result
        {
            get { return (bool?)GetValue(ResultProperty); }
            set { SetValue(ResultProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Result.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ResultProperty =
            DependencyProperty.Register("Result", typeof(bool?), typeof(LoadingResultIndicatorControl), new PropertyMetadata(null));
    }
}

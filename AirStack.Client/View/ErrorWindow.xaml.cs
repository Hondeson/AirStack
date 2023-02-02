using System.Windows;
using System.Windows.Input;

namespace AirStack.Client.View
{
    /// <summary>
    /// Interaction logic for ErrorWindow.xaml
    /// </summary>
    public partial class ErrorWindow : Window
    {
        public ErrorWindow()
        {
            InitializeComponent();
        }


        public string ErrorText
        {
            get { return (string)GetValue(ErrorTextProperty); }
            set { SetValue(ErrorTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ErrorText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ErrorTextProperty =
            DependencyProperty.Register("ErrorText", typeof(string), typeof(ErrorWindow), new PropertyMetadata("Chyba!\nVíce informací v logu aplikace"));




        public ICommand OkCommand
        {
            get { return (ICommand)GetValue(OkCommandProperty); }
            set { SetValue(OkCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OkCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OkCommandProperty =
            DependencyProperty.Register("OkCommand", typeof(ICommand), typeof(ErrorWindow), new PropertyMetadata(null));




        public bool ErrorAnimation
        {
            get { return (bool)GetValue(ErrorAnimationProperty); }
            set { SetValue(ErrorAnimationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ErrorAnimation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ErrorAnimationProperty =
            DependencyProperty.Register("ErrorAnimation", typeof(bool), typeof(ErrorWindow), new PropertyMetadata(true));


    }
}

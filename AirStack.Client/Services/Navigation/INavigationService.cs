using System.Windows.Controls;

namespace AirStack.Client.Services.Navigation
{
    public interface INavigationService
    {
        UserControl ActualView { get; set; }
        object ActualVM { get; }

        void PushView(UserControl view);
    }
}
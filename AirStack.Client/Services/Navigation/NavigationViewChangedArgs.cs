using System.Windows.Controls;

namespace AirStack.Client.Services.Navigation
{
    public class NavigationViewChangedArgs
    {
        public NavigationViewChangedArgs(UserControl newView, UserControl oldView)
        {
            NewView = newView;
            OldView = oldView;
        }

        public UserControl NewView { get; }
        public UserControl OldView { get; }
    }
}

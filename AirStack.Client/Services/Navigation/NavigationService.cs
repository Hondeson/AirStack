using AirStack.Client.ViewModel.Base;
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
        public bool CanGoBack { get => _userControls.Count > 0; }

        public event EventHandler<NavigationViewChangedArgs> ViewChanged;
        void RaiseNavigationChanged(UserControl newView, UserControl oldView)
        {
            ViewChanged?.Invoke(this, new NavigationViewChangedArgs(newView, oldView));
        }

        UserControl _ActualView;
        public UserControl ActualView
        {
            get => _ActualView;
            private set
            {
                RaiseNavigationChanged(value, _ActualView);
                Set(ref _ActualView, value);
                OnPropertyChanged(nameof(CanGoBack));
            }
        }

        public BaseVM ActualVM => (BaseVM)this.ActualView.DataContext;

        public void PushView(UserControl view)
        {
            if (view.DataContext is null)
                throw new Exception($"No ViewModel defined for {view}");

            else if (view.DataContext is not BaseVM)
                throw new Exception($"{view.DataContext.GetType().BaseType} doesnt inherit {nameof(BaseVM)}");

            if (_userControls.Count > 0
                && _userControls.Peek() == view)
                return;

            if (ActualView is not null)
                _userControls.Push(ActualView);

            this.ActualView = view;
            this.ActualVM.Initialize();
        }

        readonly HistoryStack<UserControl> _userControls = new(10);
        public void PopView()
        {
            if (_userControls.Count == 0)
                return;

            var view = _userControls.Pop();
            ActualView = view;
        }

        class HistoryStack<T>
        {
            private LinkedList<T> items = new LinkedList<T>();
            public List<T> Items => items.ToList();
            public int Count => items.Count;
            public int Capacity { get; }

            public HistoryStack(int capacity)
            {
                Capacity = capacity;
            }

            public void Push(T item)
            {
                // full
                if (items.Count == Capacity)
                {
                    // we should remove first, because some times, if we exceeded the size of the internal array
                    // the system will allocate new array.
                    items.RemoveFirst();
                    items.AddLast(item);
                }

                items.AddLast(new LinkedListNode<T>(item));
            }

            public T Pop()
            {
                if (items.Count == 0)
                    return default;

                var ls = items.Last;
                items.RemoveLast();

                return ls == null ? default : ls.Value;
            }

            public T Peek()
            {
                if (items.Count == 0)
                    return default;

                var ls = items.Last;
                return ls == null ? default : ls.Value;
            }
        }
    }
}

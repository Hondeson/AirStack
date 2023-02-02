using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AirStack.Core.Base
{
    public class ObservableObject : CommunityToolkit.Mvvm.ComponentModel.ObservableObject, INotifyPropertyChanged
    {
        public new event PropertyChangedEventHandler PropertyChanged;
        protected new void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool Set<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);

            return true;
        }

        /// <summary>
        /// Naplní objekt který ho volá propertu po propertě
        /// </summary>
        public virtual void Copy(object other)
        {
            var thisClassType = this.GetType();
            var otherClassType = other.GetType();

            if (thisClassType != otherClassType
                && thisClassType.BaseType != otherClassType)
                throw new Exception("Cannot copy different objects");

            PropertyInfo[] classProps = otherClassType.GetProperties().Where(x => x.GetSetMethod() is not null).ToArray();
            for (int i = 0; i < classProps.Length; i++)
            {
                classProps[i].SetValue(this, classProps[i].GetValue(other));
            }
        }
    }
}

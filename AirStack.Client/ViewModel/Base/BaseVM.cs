using AirStack.Core.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AirStack.Client.ViewModel.Base
{
    public class BaseVM : ObservableObject, INotifyDataErrorInfo
    {
        public BaseVM()
        {
            _ValidationHandler = new ValidationHandler();
            _ValidationHandler.ErrorsChanged += (s, e) => ErrorsChanged?.Invoke(this, e);
        }

        bool _IsBusy;
        public bool IsBusy
        {
            get => _IsBusy;
            set => Set(ref _IsBusy, value);
        }

        #region Validace
        readonly ValidationHandler _ValidationHandler;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        public bool HasErrors => _ValidationHandler.HasErrors;

        public IEnumerable GetErrors(string propertyName)
        {
            return _ValidationHandler.GetErrors(propertyName);
        }

        protected bool ValidateAllProperties(object caller)
        {
            return _ValidationHandler.ValidateAllProperties(caller);
        }

        public void ValidateProperty<T>(object caller, T value, [CallerMemberName] string propName = "")
        {
            _ValidationHandler.ValidateProperty(caller, value, propName);
        }
        #endregion
    }

    #region ValidaceHelper
    public class ValidationHandler : INotifyDataErrorInfo
    {
        readonly object _ValidationLock = new();

        readonly Dictionary<string, List<string>> _propertyNameErrorsDictionary = new();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool HasErrors => _propertyNameErrorsDictionary.Any();

        public IEnumerable GetErrors(string propertyName)
        {
            return _propertyNameErrorsDictionary.GetValueOrDefault(propertyName, new List<string>());
        }

        public bool ValidateAllProperties(object caller)
        {
            Dictionary<string, List<string>> propNameErrosDic = new();
            bool result = ValidationHelper.ValidateAllProperties(caller, out propNameErrosDic);

            ValidationNotifier(caller, propNameErrosDic);

            return result;
        }

        public void ValidateProperty<T>(object caller, T value, [CallerMemberName] string propName = "")
        {
            Dictionary<string, List<string>> propNameErrosDic = new();
            List<string> erros = new();

            ValidationHelper.ValidateProperty(caller, value, out erros, propName);

            if (erros.Count > 0)
                propNameErrosDic.Add(propName, erros);

            ValidationNotifier(caller, propNameErrosDic);
        }

        void ValidationNotifier(object caller, Dictionary<string, List<string>> propNameErrosDic)
        {
            lock (_ValidationLock)
            {
                foreach (var prop in propNameErrosDic)
                {
                    if (_propertyNameErrorsDictionary.ContainsKey(prop.Key) == false)
                        _propertyNameErrorsDictionary.Add(prop.Key, prop.Value);
                    else
                        _propertyNameErrorsDictionary[prop.Key] = prop.Value;

                    ErrorsChanged?.Invoke(caller, new DataErrorsChangedEventArgs(prop.Key));
                }

                var toRemove = _propertyNameErrorsDictionary.Where(x => propNameErrosDic.ContainsKey(x.Key) == false).Select(x => x.Key).ToList();
                foreach (var key in toRemove)
                {
                    _propertyNameErrorsDictionary.Remove(key);
                    ErrorsChanged?.Invoke(caller, new DataErrorsChangedEventArgs(key));
                }
            }
        }
    }

    public static class ValidationHelper
    {
        public static bool ValidateAllProperties(object caller, out Dictionary<string, List<string>> propertyNameErrorsDictionary)
        {
            bool result = true;
            propertyNameErrorsDictionary = new();

            var props = caller.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty);
            foreach (var prop in props)
            {
                var errorsList = new List<string>();
                var validationResult = ValidateProperty(caller, prop.GetValue(caller), out errorsList, prop.Name);

                if (validationResult == false)
                {
                    propertyNameErrorsDictionary.Add(prop.Name, errorsList);
                    result = false;
                }
            }

            return result;
        }

        public static bool ValidateProperty<T>(object caller, T value, out List<string> resultColection, [CallerMemberName] string propName = "")
        {
            var context = new ValidationContext(caller, null, null)
            {
                MemberName = propName
            };

            List<ValidationResult> validationResults = new();
            var result = Validator.TryValidateProperty(value, context, validationResults);

            resultColection = new();
            foreach (var item in validationResults)
            {
                resultColection.Add(item.ErrorMessage);
            }

            return result;
        }
    }
    #endregion
}

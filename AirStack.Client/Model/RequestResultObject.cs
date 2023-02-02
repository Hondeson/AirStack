using AirStack.Core.Base;

namespace AirStack.Client.Model
{
    public class RequestResultObject : ObservableObject
    {
        public RequestResultObject(string code)
        {
            Code = code;
            Result = null;
        }

        public string Code { get; }

        bool? _Result;
        public bool? Result
        {
            get => _Result;
            set => Set(ref _Result, value);
        }

        string _ResultMessage;
        public string ResultMessage
        {
            get => _ResultMessage;
            set => Set(ref _ResultMessage, value);
        }
    }
}

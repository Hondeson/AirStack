using System;

namespace AirStack.Client.Services.UserInput
{
    public class UserInputDataReceivedArgs : EventArgs
    {
        public UserInputDataReceivedArgs(string data)
        {
            Data = data;
        }

        public string Data { get; }
    }
}

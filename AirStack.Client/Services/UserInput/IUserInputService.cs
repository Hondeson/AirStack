using System;

namespace AirStack.Client.Services.UserInput
{
    public interface IUserInputProvider : IDisposable
    {
        event EventHandler<UserInputDataReceivedArgs> UserInputDataReceived;

        void Open();
        void Close();
    }
}
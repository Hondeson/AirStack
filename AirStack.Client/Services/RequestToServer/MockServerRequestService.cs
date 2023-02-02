using AirStack.Client.Model;
using System;
using System.Threading.Tasks;

namespace AirStack.Client.Services.RequestToServer
{
    /// <summary>
    /// Simuluje odesílání kódů na api a návrat hodnot
    /// </summary>
    internal class MockServerRequestService : IServerRequestService
    {
        Random random = new Random();
        public Task<RequestResultObject> SendRequestAsync(string code)
        {
            var num = random.Next(0, 100);

            if (num > 66)
                return Task.FromResult(new RequestResultObject(code) { Result = false, ResultMessage = "Error chyba" });

            return Task.FromResult(new RequestResultObject(code) { Result = true });
        }
    }
}

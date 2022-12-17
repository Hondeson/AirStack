using AirStack.Client.Model;
using AirStack.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirStack.Client.Services.RequestToServer
{
    internal class MockServerRequestService : IServerRequestService
    {
        Random random = new Random();
        public Task<RequestResultObject> SendRequestAsync(ItemModel item)
        {
            var num = random.Next(0, 100);

            if (num > 66)
                return Task.FromResult(new RequestResultObject(item.Code) { Result = false, ResultMessage = "Error chyba" });

            return Task.FromResult(new RequestResultObject(item.Code) { Result = true });
        }
    }
}

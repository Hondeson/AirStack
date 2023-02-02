using AirStack.Client.Model;
using AirStack.Core.Model;
using System.Threading.Tasks;

namespace AirStack.Client.Services.RequestToServer
{
    public interface IServerRequestService
    {
        Task<RequestResultObject> SendRequestAsync(string code);
    }
}
using AirStack.Core.Model;

namespace AirStack.Core.Services
{
    public interface IStatusProvider
    {
        List<StatusModel> GetAll();
    }
}
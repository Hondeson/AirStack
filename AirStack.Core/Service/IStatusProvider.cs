using AirStack.Core.Model;

namespace AirStack.Core.Service
{
    public interface IStatusProvider
    {
        List<StatusModel> GetAll();
    }
}
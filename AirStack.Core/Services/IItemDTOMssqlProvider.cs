using AirStack.Core.Model.API;

namespace AirStack.Core.Services
{
    public interface IItemDTOProvider
    {
        GetItemDTOList Get(long offset, long fetch);
    }
}
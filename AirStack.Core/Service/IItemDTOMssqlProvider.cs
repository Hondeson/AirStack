using AirStack.Core.Model;
using AirStack.Core.Model.API;

namespace AirStack.Core.Service
{
    public interface IItemDTOProvider
    {
        List<GetItemDTO> Get(long offset, long fetch, List<StatusEnum> statuses, DateTime? productionFrom, DateTime? productionTo);
        long GetCount(List<StatusEnum> statuses, DateTime? productionFrom, DateTime? productionTo);
    }
}
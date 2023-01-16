using AirStack.Core.Model;

namespace AirStack.Core.Service
{
    public interface IItemHistoryProvider
    {
        bool Create(ItemHistoryModel model);
        ItemHistoryModel Get(long id);
        List<ItemHistoryModel> GetByItemId(long itemId);
    }
}
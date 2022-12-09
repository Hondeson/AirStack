using AirStack.Core.Model;

namespace AirStack.Core.Services
{
    public interface IItemHistoryProvider
    {
        bool Create(ItemHistoryModel model);
        ItemHistoryModel Get(long id);
        List<ItemHistoryModel> GetByItemId(long itemId);
    }
}
using AirStack.Core.Model;

namespace AirStack.Core.Services
{
    public interface IItemProvider
    {
        bool Create(ItemModel item);
        List<ItemModel> Filter(string codeFilterString);
        ItemModel Get(long id);
        ItemModel Get(string code);
        bool Update(ItemModel item);
    }
}
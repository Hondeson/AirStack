using AirStack.Core.Model;

namespace AirStack.Core.Services
{
    public interface IItemProvider
    {
        bool Create(ItemModel item);
        List<ItemModel> Filter(string codeFilterString);
        ItemModel Get(long id);
        ItemModel GetByCode(string code);
        ItemModel GetByParentCode(string parentCode);
        bool Update(ItemModel item);
    }
}
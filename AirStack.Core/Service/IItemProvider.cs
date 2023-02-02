using AirStack.Core.Model;

namespace AirStack.Core.Service
{
    public interface IItemProvider
    {
        bool Create(ItemModel item);
        List<ItemModel> Filter(string codeFilterString);
        ItemModel Get(long id);
        ItemModel GetByCode(string code);
        ItemModel GetByParentCode(string parentCode);
        bool Update(ItemModel item);
        bool Delete(long id);
    }
}
using AirStack.Core.Model;

namespace AirStack.API.DTO
{
    public class UpdateItemDTO
    {
        public ItemModel Item { get; set; }
        public ItemHistoryModel History { get; set; }
    }
}

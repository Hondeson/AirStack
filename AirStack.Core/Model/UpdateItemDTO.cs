using AirStack.Core.Model;

namespace AirStack.Core.Model
{
    public class UpdateItemDTO
    {
        public UpdateItemDTO() { }

        public UpdateItemDTO(ItemModel item, StatusEnum state)
        {
            this.Item = item;
            this.History = new ItemHistoryModel() { ItemID = this.Item.ID, StatusID = (long)state, CreatedAt = DateTime.Now };
        }

        public ItemModel Item { get; set; }
        public ItemHistoryModel History { get; set; }
    }
}

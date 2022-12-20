namespace AirStack.Core.Model.API
{
    public class UpdateItemDTO
    {
        public UpdateItemDTO() { }

        public UpdateItemDTO(ItemModel item, StatusEnum state)
        {
            Item = item;
            History = new ItemHistoryModel() { ItemID = Item.ID, StatusID = (long)state, CreatedAt = DateTime.Now };
        }

        public ItemModel Item { get; set; }
        public ItemHistoryModel History { get; set; }
    }
}

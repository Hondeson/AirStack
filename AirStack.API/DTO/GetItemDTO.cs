using AirStack.Core.Model;

namespace AirStack.API.DTO
{
    public class GetItemDTO
    {
        public ItemModel Item { get; set; }
        public List<GetItemHistoryModel> HistoryList { get; set; }
    }

    public class GetItemHistoryModel
    {
        public long ID { get; set; }
        public StatusModel Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

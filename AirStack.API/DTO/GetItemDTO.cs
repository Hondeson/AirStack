using AirStack.Core.Model;

namespace AirStack.API.DTO
{
    public class GetItemDTO
    {
        public GetItemDTO() { }

        public GetItemDTO(ItemModel item, List<ItemHistoryModel> historyList)
        {
            SetItemData(item);
            SetHistoryData(historyList);
        }

        public long ID { get; set; }
        public string Code { get; set; }
        public string ParentCode { get; set; }
        public string ActualStatus { get; set; }

        public DateTime? ProductionDate { get; set; }
        public DateTime? DispatchedDate { get; set; }
        public DateTime? TestsDate { get; set; }
        public DateTime? ComplaintDate { get; set; }
        public DateTime? ComplaintDateToSupplier { get; set; }

        public void SetItemData(ItemModel item)
        {
            this.ID = item.ID;
            this.Code = item.Code;
            this.ParentCode = item.ParentCode;
        }

        public void SetHistoryData(List<ItemHistoryModel> historyList)
        {
            if (historyList.Count == 0)
                return;

            historyList = historyList.OrderByDescending(x => x.CreatedAt).ToList();
            this.ActualStatus = ((StatusEnum)historyList.First().StatusID).ToString();

            foreach (var item in historyList)
            {
                StatusEnum status = (StatusEnum)item.StatusID;
                switch (status)
                {
                    case StatusEnum.Production:
                        ProductionDate = item.CreatedAt;
                        break;
                    case StatusEnum.Dispatched:
                        DispatchedDate = item.CreatedAt;
                        break;
                    case StatusEnum.Tests:
                        TestsDate = item.CreatedAt;
                        break;
                    case StatusEnum.Complaint:
                        ComplaintDate = item.CreatedAt;
                        break;
                    case StatusEnum.ComplaintToSupplier:
                        ComplaintDate = item.CreatedAt;
                        break;
                }
            }
        }
    }
}

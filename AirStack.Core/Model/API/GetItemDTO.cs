using AirStack.Core.Model;

namespace AirStack.Core.Model.API
{
    public class GetItemDTO
    {
        public GetItemDTO() { }

        public GetItemDTO(ItemModel item, List<ItemHistoryModel> historyList)
        {
            SetItemData(item);
            SetHistoryData(historyList);
            LoadActualStatus();
        }

        public long ID { get; set; }
        public string Code { get; set; }
        public string ParentCode { get; set; }
        public string ActualStatus { get; set; }

        public DateTime? ProductionDate { get; set; }
        public DateTime? DispatchedDate { get; set; }
        public DateTime? TestsDate { get; set; }
        public DateTime? ComplaintDate { get; set; }
        public DateTime? ComplaintToSupplierDate { get; set; }

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

        /// <summary>
        /// Podle properties najde nejnovější datum a jeho StatusEnum hodnotu jako ActualStatus
        /// </summary>
        public void LoadActualStatus()
        {
            var dic = new Dictionary<string, DateTime?>();
            var props = this.GetType().GetProperties().Where(x => x.PropertyType == typeof(DateTime?));
            foreach (var dateProp in props)
            {
                dic.Add(dateProp.Name, (DateTime?)dateProp.GetValue(this));
            }

            if (dic.Count == 0)
                return;

            var newestDatePropName = dic.MaxBy(x => x.Value).Key;
            var enumName = newestDatePropName.Replace("Date", "");
            if (Enum.TryParse<StatusEnum>(enumName, out _) == false)
            {
                throw new Exception($"ModelError: {this.GetType().Name} Date property doesnt have matching enum value");
            }

            this.ActualStatus = enumName;
        }
    }
}

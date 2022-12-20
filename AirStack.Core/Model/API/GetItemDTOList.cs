namespace AirStack.Core.Model.API
{
    public class GetItemDTOList
    {
        public GetItemDTOList(List<GetItemDTO> items, long totalCount)
        {
            Items = items;
            TotalCount = totalCount;
        }

        public List<GetItemDTO> Items { get; }
        public long TotalCount { get; }
    }
}

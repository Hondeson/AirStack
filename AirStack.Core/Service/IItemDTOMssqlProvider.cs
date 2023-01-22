using AirStack.Core.Model;
using AirStack.Core.Model.API;

namespace AirStack.Core.Service
{
    public interface IItemDTOProvider
    {
        List<GetItemDTO> Get(
            long offset, long fetch,
            List<StatusEnum> statuses,
            string? codeLike, string? parentCodeLike,
            DateTimeOffset? productionFrom, DateTimeOffset? productionTo,
            DateTimeOffset? dispatchedFrom, DateTimeOffset? dispatchedTo,
            DateTimeOffset? testsFrom, DateTimeOffset? testsTo,
            DateTimeOffset? complaintFrom, DateTimeOffset? complaintTo,
            DateTimeOffset? complaintSuplFrom, DateTimeOffset? complaintSuplTo);

        long GetCount(
            List<StatusEnum> statuses,
            string? codeLike, string? parentCodeLike,
            DateTimeOffset? productionFrom, DateTimeOffset? productionTo,
            DateTimeOffset? dispatchedFrom, DateTimeOffset? dispatchedTo,
            DateTimeOffset? testsFrom, DateTimeOffset? testsTo,
            DateTimeOffset? complaintFrom, DateTimeOffset? complaintTo,
            DateTimeOffset? complaintSuplFrom, DateTimeOffset? complaintSuplTo);
    }
}
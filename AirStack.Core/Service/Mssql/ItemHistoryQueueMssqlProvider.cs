using AirStack.Core.Connection;
using Dapper;

namespace AirStack.Core.Service.Mssql
{
    public class ItemHistoryQueueMssqlProvider : IItemHistoryQueueProvider
    {
        readonly ISqlAdapter _slq;
        public ItemHistoryQueueMssqlProvider(ISqlAdapter slq)
        {
            _slq = slq;
        }

        string c_ExecProcessQueueItem = @"[ProcessQueueItem]";
        public void Process()
        {
            using (var con = _slq.Connection())
            {
                con.Execute(c_ExecProcessQueueItem);
            }
        }
    }
}

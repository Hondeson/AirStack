using AirStack.Core.Connection;
using AirStack.Core.Model;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

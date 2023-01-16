using AirStack.Core.Connection;
using AirStack.Core.Helper.Dapper;
using AirStack.Core.Model;
using AirStack.Core.Model.API;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace AirStack.Core.Service.Mssql
{
    public class ItemDTOMssqlProvider : IItemDTOProvider
    {
        readonly ISqlAdapter _sql;
        public ItemDTOMssqlProvider(ISqlAdapter sqlAdapter)
        {
            _sql = sqlAdapter;
        }

        const string c_GetItemDTOProc = "[dbo].[GetItemDTO]";
        public List<GetItemDTO> Get(long offset, long fetch, List<StatusEnum> statuses, DateTime? productionFrom, DateTime? productionTo)
        {
            using (var con = _sql.Connect())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Offset", offset);
                parameters.Add("@Fetch", fetch);
                parameters.AddStringList("@Statuses", statuses.Select(x => x.ToString()).ToList());
                parameters.Add("@ProductionFrom", productionFrom);
                parameters.Add("@ProductionTo", productionTo);

                return con.Query<GetItemDTO>(c_GetItemDTOProc, parameters, commandType: CommandType.StoredProcedure).ToList();
            }
        }

        const string c_GetItemsCountProc =
            @"exec [dbo].[GetItemCount] @Statuses, @ProductionFrom, @ProductionTo";
        public long GetCount(List<StatusEnum> statuses, DateTime? productionFrom, DateTime? productionTo)
        {
            throw new NotImplementedException();
            using (var con = _sql.Connect())
            {
                return con.ExecuteScalar<long>(c_GetItemsCountProc, new { ProductionFrom = productionFrom, ProductionTo = productionTo });
            }
        }
    }
}

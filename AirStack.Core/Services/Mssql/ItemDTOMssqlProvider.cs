using AirStack.Core.Connection;
using AirStack.Core.Model.API;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirStack.Core.Services.Mssql
{
    public class ItemDTOMssqlProvider : IItemDTOProvider
    {
        readonly ISqlAdapter _sql;
        public ItemDTOMssqlProvider(ISqlAdapter sqlAdapter)
        {
            _sql = sqlAdapter;
        }

        const string c_GetItemByIDQuery =
            @"exec [dbo].[GetItemDTO] @Offset, @Fetch";

        const string c_GetItemsCount =
            @"select count(1) from Item";
        public GetItemDTOList Get(long offset, long fetch)
        {
            using (var con = _sql.Connect())
            {
                var itemList = con.Query<GetItemDTO>(c_GetItemByIDQuery, new { Offset = offset, Fetch = fetch }).ToList();
                long totalItemsCount = con.ExecuteScalar<long>(c_GetItemsCount);

                return new GetItemDTOList(itemList, totalItemsCount);
            }
        }
    }
}

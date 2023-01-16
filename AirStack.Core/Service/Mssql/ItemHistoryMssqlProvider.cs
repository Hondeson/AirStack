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
    public class ItemHistoryMssqlProvider : IItemHistoryProvider
    {
        readonly ISqlAdapter _sql;
        public ItemHistoryMssqlProvider(ISqlAdapter sql)
        {
            _sql = sql;
        }

        const string c_GetQuery =
            @"select * from ItemHistory where ID = @ID";
        public ItemHistoryModel Get(long id)
        {
            using (var con = _sql.Connect())
            {
                object param = new { ID = id };
                return con.QueryFirst<ItemHistoryModel>(c_GetQuery, param);
            }
        }

        const string c_GetByItemIdQuery =
            @"select * from ItemHistory
                where ItemID = @ItemID";
        public List<ItemHistoryModel> GetByItemId(long itemId)
        {
            using (var con = _sql.Connect())
            {
                object param = new { ItemID = itemId };
                return con.Query<ItemHistoryModel>(c_GetByItemIdQuery, param).ToList();
            }
        }

        const string c_CreateQuery =
            @"insert into ItemHistory
                (ItemID, StatusID, CreatedAt) values
                (@ItemID, @StatusID, @CreatedAt);
                
                SELECT SCOPE_IDENTITY();";
        public bool Create(ItemHistoryModel model)
        {
            if (model.ItemID < 1)
                throw new ArgumentException("Creating Item history requires existing Item!");

            if (model.StatusID < 1)
                throw new ArgumentException("Creating Item history requires valid statusID!");

            using (var con = _sql.Connect())
            {
                object param = new { model.ItemID, model.StatusID, model.CreatedAt };
                model.ID = con.ExecuteScalar<long>(c_CreateQuery, param);

                return model.ID > 0;
            }
        }
    }
}

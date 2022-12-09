using AirStack.Core.Connection;
using AirStack.Core.Model;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirStack.Core.Services
{
    public class ItemMssqlProvider : IItemProvider
    {
        readonly ISqlAdapter _sql;
        public ItemMssqlProvider(ISqlAdapter sql)
        {
            _sql = sql;
        }

        public ItemModel Get(long id)
        {
            return Get(id, null)?.FirstOrDefault();
        }

        public ItemModel Get(string code)
        {
            return Get(-1, code: code)?.FirstOrDefault();
        }

        public List<ItemModel> Filter(string codeFilterString)
        {
            return Get(-1, codeFilterString: codeFilterString);
        }

        const string c_GetItemProc =
            @"exec [dbo].[GetItem] @ID, @Code, @ParentCode, @CodeLike";
        List<ItemModel> Get(long id, string code = null, string parentCode = null, string codeFilterString = null)
        {
            using (var con = _sql.Connect())
            {
                object param = new { ID = id, Code = code, ParentCode = parentCode, CodeLike = codeFilterString };
                return con.Query<ItemModel>(c_GetItemProc, param).ToList();
            }
        }

        const string c_CreateItemQuery =
            @"insert into [dbo].[Item] 
                ([Code], [ParentCode]) values
                (@Code, @ParentCode);
                
                SELECT SCOPE_IDENTITY();";
        public bool Create(ItemModel item)
        {
            using (var con = _sql.Connect())
            {
                object param = new { Code = item.Code, ParentCode = item.ParentCode };
                item.ID = con.ExecuteScalar<long>(c_CreateItemQuery, param);

                return item.ID > 0;
            }
        }

        const string c_UpdateItemQuery =
            @"update [dbo].[Item]
                set [Code] = @Code, [ParentCode] = @ParentCode";
        public bool Update(ItemModel item)
        {
            using (var con = _sql.Connect())
            {
                object param = new { Code = item.Code, ParentCode = item.ParentCode };
                int res = con.Execute(c_UpdateItemQuery, param);

                return res > 0;
            }
        }
    }
}

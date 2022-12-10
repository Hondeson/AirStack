using AirStack.Core.Connection;
using AirStack.Core.Model;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirStack.Core.Services.Mssql
{
    public class StatusMssqlProvider : IStatusProvider
    {
        readonly ISqlAdapter _sql;
        public StatusMssqlProvider(ISqlAdapter sql)
        {
            _sql = sql;
        }

        const string c_GetAllQuery =
            @"select * from Status";
        public List<StatusModel> GetAll()
        {
            using (var con = _sql.Connect())
            {
                return con.Query<StatusModel>(c_GetAllQuery).ToList();
            }
        }
    }
}

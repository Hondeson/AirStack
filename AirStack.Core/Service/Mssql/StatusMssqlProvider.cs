using AirStack.Core.Connection;
using AirStack.Core.Model;
using Dapper;

namespace AirStack.Core.Service.Mssql
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
            using (var con = _sql.Connection())
            {
                return con.Query<StatusModel>(c_GetAllQuery).ToList();
            }
        }
    }
}

using AirStack.Core.Connection;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirStack.Core.Service.Mssql
{
    public class SettingsMssqlProvider : ISettingsProvider
    {
        readonly ISqlAdapter _sql;
        public SettingsMssqlProvider(ISqlAdapter sql)
        {
            _sql = sql;
        }

        const string c_GetCodeRegexesQuery =
            @"select [value] from Settings where [Name] like 'CodeRegex_%'";
        public List<string> GetCodeRegexes()
        {
            using (var con = _sql.Connection())
            {
                return con.Query<string>(c_GetCodeRegexesQuery).ToList();
            }
        }
    }
}

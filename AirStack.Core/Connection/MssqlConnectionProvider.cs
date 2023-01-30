using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirStack.Core.Connection
{
    public class MssqlConnectionProvider : ISqlAdapter
    {
        string _conString = "";
        public MssqlConnectionProvider(string conString)
        {
            _conString = conString;
        }

        public IDbConnection Connection()
        {
            return new SqlConnection(_conString);
        }
    }
}

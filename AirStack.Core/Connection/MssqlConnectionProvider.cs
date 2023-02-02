using Microsoft.Data.SqlClient;
using System.Data;

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

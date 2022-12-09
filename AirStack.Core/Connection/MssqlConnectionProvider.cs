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
#if DEBUG
        const string c_ConnectionString = "Server={0};Database=AirStack;Trusted_Connection=True;";
#else
        const string c_ConnectionString = "Server={0};Database=AirStack;User Id={1};Password={2};";
#endif
        string _dbServer = "";
        public MssqlConnectionProvider(string dbServer)
        {
            _dbServer = dbServer;
#if !DEBUG
            throw new NotImplementedException("Con string braško");
#endif
        }

        public IDbConnection Connect()
        {
            return new SqlConnection(string.Format(c_ConnectionString, _dbServer));
        }
    }
}

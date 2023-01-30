using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirStack.Core.Connection
{
    public interface ISqlAdapter
    {
        IDbConnection Connection();
    }
}

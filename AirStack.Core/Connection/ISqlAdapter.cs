using System.Data;

namespace AirStack.Core.Connection
{
    public interface ISqlAdapter
    {
        IDbConnection Connection();
    }
}

using System.Collections.Generic;
using System.Data.SqlClient;

namespace LambdaSql.Filter.SqlFilterItem
{
    internal interface ISqlFilterItem
    {
        string Expression { get; }
        IEnumerable<SqlParameter> Parameters { get; }
    }
}
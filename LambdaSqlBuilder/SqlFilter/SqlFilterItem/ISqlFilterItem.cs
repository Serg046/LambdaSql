using System.Collections.Generic;
using System.Data.SqlClient;

namespace LambdaSqlBuilder.SqlFilter.SqlFilterItem
{
    internal interface ISqlFilterItem
    {
        string Expression { get; }
        IEnumerable<SqlParameter> Parameters { get; }
    }
}
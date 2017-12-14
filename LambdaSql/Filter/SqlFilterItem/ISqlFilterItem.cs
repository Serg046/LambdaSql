using System.Collections.Generic;
using System.Data.Common;

namespace LambdaSql.Filter.SqlFilterItem
{
    internal interface ISqlFilterItem
    {
        string Expression { get; }
        IEnumerable<DbParameter> Parameters { get; }
    }
}
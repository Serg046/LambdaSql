using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace LambdaSql.Filter.SqlFilterItem
{
    internal class ConstSqlFilterItem : ISqlFilterItem
    {
        public ConstSqlFilterItem(string value)
        {
            Expression = value;
        }

        public string Expression { get; }

        public IEnumerable<DbParameter> Parameters { get; } = Enumerable.Empty<DbParameter>();
    }
}

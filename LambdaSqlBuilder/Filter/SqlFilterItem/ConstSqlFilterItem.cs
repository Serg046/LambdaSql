using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace LambdaSqlBuilder.Filter.SqlFilterItem
{
    internal class ConstSqlFilterItem : ISqlFilterItem
    {
        public ConstSqlFilterItem(string value)
        {
            Expression = value;
        }

        public string Expression { get; }

        public IEnumerable<SqlParameter> Parameters => Enumerable.Empty<SqlParameter>();
    }
}

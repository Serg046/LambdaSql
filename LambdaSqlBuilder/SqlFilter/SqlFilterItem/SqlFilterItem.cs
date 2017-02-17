using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace LambdaSqlBuilder.SqlFilter.SqlFilterItem
{
    internal class SqlFilterItem : ISqlFilterItem
    {
        public SqlFilterItem(string expression, params SqlFilterParameter[] args)
        {
            Expression = string.Format(expression, args);
            Parameters = args.Select(p => p.Parameter).Where(p => p != null);
        }

        public string Expression { get; } 

        public IEnumerable<SqlParameter> Parameters { get; }
    }
}

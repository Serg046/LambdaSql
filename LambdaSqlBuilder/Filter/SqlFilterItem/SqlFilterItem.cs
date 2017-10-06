﻿using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace LambdaSqlBuilder.Filter.SqlFilterItem
{
    internal class SqlFilterItem : ISqlFilterItem
    {
        private readonly string _expression;
        private readonly SqlFilterParameter[] _sqlFilterParameters;

        public SqlFilterItem(string expression, params SqlFilterParameter[] args)
        {
            _expression = expression;
            _sqlFilterParameters = args;
            Parameters = args.Select(p => p.Parameter).Where(p => p != null);
        }

        public string Expression => string.Format(_expression, _sqlFilterParameters);

        public IEnumerable<SqlParameter> Parameters { get; }
    }
}

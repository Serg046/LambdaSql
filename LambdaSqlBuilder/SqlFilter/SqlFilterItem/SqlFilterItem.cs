namespace LambdaSqlBuilder.SqlFilter.SqlFilterItem
{
    internal class SqlFilterItem : ISqlFilterItem
    {
        private readonly SqlFilterConfiguration _configuration;
        private readonly string _expression;
        private readonly object[] _args;

        public SqlFilterItem(SqlFilterConfiguration configuration, string expression, params object[] args)
        {
            _configuration = configuration;
            _expression = expression;
            _args = args;
        }

        public override string ToString()
        {
            if (_configuration.WithoutAliases && _args != null && _args.Length > 0)
            {
                var args = new object[_args.Length];
                for (var i = 0; i < _args.Length; i++)
                {
                    var field = _args[i] as ISqlField;
                    args[i] = field?.Name ?? _args[i];
                }
                return string.Format(_expression, args);
            }
            return string.Format(_expression, _args);
        }
    }
}

namespace LambdaSqlBuilder.SqlFilter.SqlFilterItem
{
    internal class ConstSqlFilterItem : ISqlFilterItem
    {
        private readonly string _value;

        public ConstSqlFilterItem(string value)
        {
            _value = value;
        }

        public override string ToString() => _value;
    }
}

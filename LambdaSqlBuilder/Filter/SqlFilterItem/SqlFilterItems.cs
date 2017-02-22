namespace LambdaSqlBuilder.Filter.SqlFilterItem
{
    internal static class SqlFilterItems
    {
        public static SqlFilterItemFunc And = config => new ConstSqlFilterItem(" AND ");
        public static SqlFilterItemFunc Or = config => new ConstSqlFilterItem(" OR ");
        public static SqlFilterItemFunc Build(string value) => config => new ConstSqlFilterItem(value);
    }
}

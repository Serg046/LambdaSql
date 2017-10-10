namespace LambdaSql.Filter.SqlFilterItem
{
    internal static class SqlFilterItems
    {
        public static SqlFilterItemCallback And = config => new ConstSqlFilterItem(" AND ");
        public static SqlFilterItemCallback Or = config => new ConstSqlFilterItem(" OR ");
        public static SqlFilterItemCallback Build(string value) => config => new ConstSqlFilterItem(value);
    }
}

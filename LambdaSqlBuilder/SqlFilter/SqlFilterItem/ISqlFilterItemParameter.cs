namespace LambdaSqlBuilder.SqlFilter.SqlFilterItem
{
    internal interface ISqlFilterItemParameter
    {
        string ToString(bool withoutAliases, bool rawSql);
    }
}

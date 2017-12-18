using System.Text;

namespace LambdaSql.QueryBuilder
{
    internal class SqlSelectQueryBuilder : SqlSelectQueryBuilderBase
    {
        private readonly string _tableName;

        public SqlSelectQueryBuilder(string tableName)
        {
            _tableName = tableName;

        }

        public override string Build(SqlSelectInfo info, bool parametric)
        {
            CheckAsAliases(info);
            var sb = new StringBuilder("SELECT").Append(SEPARATOR_WITH_OFFSET)
                .Append(GetSelectedFields(info)).Append(SEPARATOR)
                .Append("FROM").Append(SEPARATOR_WITH_OFFSET)
                .Append(_tableName)
                .Append(" ").Append(info.Alias.Value);
            AppendJoins(sb, info);
            AppendWhere(sb, info, parametric);
            AppendGroupByFields(sb, info);
            AppendHaving(sb, info, parametric);
            AppendOrderByFields(sb, info);
            return GetQueryString(sb);
        }
    }
}

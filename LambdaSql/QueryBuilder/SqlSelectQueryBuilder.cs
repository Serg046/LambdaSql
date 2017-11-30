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

        public override string Build(SqlSelectInfo info)
        {
            CheckAsAliases(info);
            var sb = new StringBuilder("SELECT").Append(SEPARATOR_WITH_OFFSET)
                .Append(GetSelectedFields(info)).Append(SEPARATOR)
                .Append("FROM").Append(SEPARATOR_WITH_OFFSET)
                .Append(_tableName)
                .Append(" ").Append(info.Alias.Value);
            AppendJoins(sb, info);
            AppendFilter(sb, info, "WHERE", sInfo => sInfo.Where());
            AppendFields(sb, info, "GROUP BY", sInfo => sInfo.GroupByFields());
            AppendFilter(sb, info, "HAVING", sInfo => sInfo.Having());
            AppendFields(sb, info, "ORDER BY", sInfo => sInfo.OrderByFields());
            return sb.ToString();
        }
    }
}

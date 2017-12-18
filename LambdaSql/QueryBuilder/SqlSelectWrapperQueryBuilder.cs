using System;
using System.Linq;
using System.Text;

namespace LambdaSql.QueryBuilder
{
    internal class SqlSelectWrapperQueryBuilder : SqlSelectQueryBuilderBase
    {
        private readonly ISqlSelect _innerSqlSelect;

        public SqlSelectWrapperQueryBuilder(ISqlSelect innerSqlSelect)
        {
            _innerSqlSelect = innerSqlSelect;

        }

        public override string Build(SqlSelectInfo info, bool parametric)
        {
            CheckAsAliases(info);
            CheckSelectedFields(info);
            var innerSql = parametric ? _innerSqlSelect.ParametricSql : _innerSqlSelect.RawSql;
            var sb = new StringBuilder("SELECT").Append(SEPARATOR_WITH_OFFSET)
                .Append(GetSelectedFields(info)).Append(SEPARATOR)
                .Append("FROM")
                .Append(SEPARATOR).Append("(").Append(SEPARATOR_WITH_OFFSET)
                .Append(innerSql.Replace(SEPARATOR, SEPARATOR_WITH_OFFSET))
                .Append(SEPARATOR).Append(") AS ").Append(info.Alias.Value);
            AppendJoins(sb, info);
            AppendWhere(sb, info, parametric);
            AppendGroupByFields(sb, info);
            AppendHaving(sb, info, parametric);
            AppendOrderByFields(sb, info);
            return GetQueryString(sb);
        }

        private void CheckSelectedFields(SqlSelectInfo info)
        {
            foreach (var selectField in info.SelectFields())
            {
                if (!_innerSqlSelect.Info.SelectFields().Any(f => f.Name == selectField.Name
                                                           && f.EntityType == selectField.EntityType))
                {
                    throw new InvalidOperationException(
                        $"'{selectField}' is not set in the inner select query.");
                }
            }
        }
    }
}

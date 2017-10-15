using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using GuardExtensions;
using LambdaSql.Field;
using LambdaSql.Filter;

namespace LambdaSql
{
    public class SqlSelect : SqlSelectBase, ISqlSelect
    {
        private readonly ISqlSelect _innerSqlSelect;

        public SqlSelect(ISqlSelect innerSqlSelect, ISqlAlias alias) : base(alias)
        {
            Guard.IsNotNull(innerSqlSelect, alias);
            _innerSqlSelect = innerSqlSelect;
        }

        private SqlSelect(ISqlSelect innerSqlSelect, SqlSelectInfo info) : base(info)
        {
            _innerSqlSelect = innerSqlSelect;
        }

        private SqlSelect CreateSqlSelect(SqlSelectInfo info) => new SqlSelect(_innerSqlSelect, info);

        public string CommandText
        {
            get
            {
                CheckAsAliases();
                CheckSelectedFields();
                var sb = new StringBuilder("SELECT").Append(SEPARATOR_WITH_OFFSET);
                SelectedFields(sb);
                sb.Append("FROM")
                    .Append(SEPARATOR).Append("(").Append(SEPARATOR_WITH_OFFSET)
                    .Append(_innerSqlSelect.CommandText.Replace(SEPARATOR, SEPARATOR_WITH_OFFSET))
                    .Append(SEPARATOR).Append(") AS ").Append(Info.Alias.Value);
                var groupByFields = Info.GroupByFields();
                if (groupByFields.Count > 0)
                {
                    sb.Append(SEPARATOR).Append("GROUP BY")
                        .Append(SEPARATOR_WITH_OFFSET).Append(string.Join(", ", groupByFields.Select(f => f.ShortView)));
                }
                return sb.ToString();
            }
        }

        private void CheckSelectedFields()
        {
            foreach (var selectField in Info.SelectFields())
            {
                if (!_innerSqlSelect.SelectFields.Any(f => f.Name == selectField.Name
                                                           && f.EntityType == selectField.EntityType))
                {
                    throw new InvalidOperationException(
                        $"'{selectField}' is not set in the inner select query.");
                }
            }
        }

        public override string ToString()
        {
            return CommandText;
        }

        public Type EntityType => null;

        ISqlSelect ISqlSelect.Top(int top) => Top(top);
        public SqlSelect Top(int top)
        {
            if (top < 1)
            {
                throw new InvalidOperationException("The value must be greather than 0");
            }
            return new SqlSelect(_innerSqlSelect, Info.Top(top));
        }

        public SqlSelect Distinct()
        {
            return Distinct(true);
        }

        ISqlSelect ISqlSelect.Distinct(bool isDistinct) => Distinct(isDistinct);
        public SqlSelect Distinct(bool isDistinct)
        {
            return CreateSqlSelect(Info.Distinct(isDistinct));
        }

        //-------------------------------------------------------------------------

        ISqlSelect ISqlSelect.AddFields(ISqlField[] fields) => AddFields(fields);
        public SqlSelect AddFields(params ISqlField[] fields)
        {
            Guard.IsNotNull(fields);
            return CreateSqlSelect(Info.SelectFields(fields.Select(f =>
            {
                f.Alias = Info.Alias;
                return f;
            }).ToArray()));
        }

        public SqlSelect AddFields<TEntity>(params Expression<Func<TEntity, object>>[] fields)
        {
            Guard.IsNotNull(fields);
            return AddFields(CreateSqlFields(Info.Alias, fields));
        }

        ISqlSelect ISqlSelect.AddFields<TEntity>(SqlAlias<TEntity> alias, params Expression<Func<TEntity, object>>[] fields)
            => AddFields(fields);

        //-------------------------------------------------------------------------

        ISqlSelect ISqlSelect.GroupBy(ISqlField[] fields) => GroupBy(fields);
        public SqlSelect GroupBy(params ISqlField[] fields)
        {
            Guard.IsNotNull(fields);
            return CreateSqlSelect(Info.GroupByFields(fields.Select(f =>
            {
                f.Alias = Info.Alias;
                return f;
            }).ToArray()));
        }

        public SqlSelect GroupBy<TEntity>(params Expression<Func<TEntity, object>>[] fields)
        {
            Guard.IsNotNull(fields);
            return GroupBy(CreateSqlFields(Info.Alias, fields));
        }

        ISqlSelect ISqlSelect.GroupBy<TEntity>(SqlAlias<TEntity> alias, params Expression<Func<TEntity, object>>[] fields)
            => GroupBy(fields);

        //-------------------------------------------------------------------------

        ISqlSelect ISqlSelect.OrderBy(ISqlField[] fields) => OrderBy(fields);
        public SqlSelect OrderBy(params ISqlField[] fields)
        {
            Guard.IsNotNull(fields);
            return CreateSqlSelect(Info.OrderByFields(fields.Select(f =>
            {
                f.Alias = Info.Alias;
                return f;
            }).ToArray()));
        }

        public SqlSelect OrderBy<TEntity>(params Expression<Func<TEntity, object>>[] fields)
        {
            Guard.IsNotNull(fields);
            return OrderBy(CreateSqlFields(Info.Alias, fields));
        }

        ISqlSelect ISqlSelect.OrderBy<TEntity>(SqlAlias<TEntity> alias, params Expression<Func<TEntity, object>>[] fields)
            => OrderBy(fields);

        //-------------------------------------------------------------------------

        public SqlSelect InnerJoin<TLeft, TJoin>(Expression<Func<TLeft, TJoin, bool>> condition,
            SqlAlias<TLeft> leftAlias = null, SqlAlias<TJoin> joinAlias = null)
        {
            Guard.IsNotNull(condition);
            if (leftAlias == null)
                leftAlias = MetadataProvider.AliasFor<TLeft>();
            if (joinAlias == null)
                joinAlias = MetadataProvider.AliasFor<TJoin>();

            return InnerJoin(GetJoinFilter(condition.Body as BinaryExpression, leftAlias, joinAlias), joinAlias);
        }

        public SqlSelect InnerJoin<TJoin>(ISqlFilter condition, SqlAlias<TJoin> joinAlias = null)
        {
            Guard.IsNotNull(condition);
            return CreateSqlSelect(Join(JoinType.Inner, condition, joinAlias));
        }

        public SqlSelect LeftJoin<TLeft, TJoin>(Expression<Func<TLeft, TJoin, bool>> condition,
            SqlAlias<TLeft> leftAlias = null, SqlAlias<TJoin> joinAlias = null)
        {
            Guard.IsNotNull(condition);
            if (leftAlias == null)
                leftAlias = MetadataProvider.AliasFor<TLeft>();
            if (leftAlias == null)
                joinAlias = MetadataProvider.AliasFor<TJoin>();

            return LeftJoin(GetJoinFilter(condition.Body as BinaryExpression, leftAlias, joinAlias), joinAlias);
        }

        public SqlSelect LeftJoin<TJoin>(ISqlFilter condition, SqlAlias<TJoin> joinAlias = null)
        {
            Guard.IsNotNull(condition);
            return CreateSqlSelect(Join(JoinType.Left, condition, joinAlias));
        }

        public SqlSelect RightJoin<TLeft, TJoin>(Expression<Func<TLeft, TJoin, bool>> condition,
            SqlAlias<TLeft> leftAlias = null, SqlAlias<TJoin> joinAlias = null)
        {
            Guard.IsNotNull(condition);
            if (leftAlias == null)
                leftAlias = MetadataProvider.AliasFor<TLeft>();
            if (joinAlias == null)
                joinAlias = MetadataProvider.AliasFor<TJoin>();

            return RightJoin(GetJoinFilter(condition.Body as BinaryExpression, leftAlias, joinAlias), joinAlias);
        }

        public SqlSelect RightJoin<TJoin>(ISqlFilter condition, SqlAlias<TJoin> joinAlias = null)
        {
            Guard.IsNotNull(condition);
            return CreateSqlSelect(Join(JoinType.Right, condition, joinAlias));
        }

        public SqlSelect FullJoin<TLeft, TJoin>(Expression<Func<TLeft, TJoin, bool>> condition,
            SqlAlias<TLeft> leftAlias = null, SqlAlias<TJoin> joinAlias = null)
        {
            Guard.IsNotNull(condition);
            if (leftAlias == null)
                leftAlias = MetadataProvider.AliasFor<TLeft>();
            if (joinAlias == null)
                joinAlias = MetadataProvider.AliasFor<TJoin>();

            return FullJoin(GetJoinFilter(condition.Body as BinaryExpression, leftAlias, joinAlias), joinAlias);
        }

        public SqlSelect FullJoin<TJoin>(ISqlFilter condition, SqlAlias<TJoin> joinAlias = null)
        {
            Guard.IsNotNull(condition);
            return CreateSqlSelect(Join(JoinType.Full, condition, joinAlias));
        }

        ISqlSelect ISqlSelect.Join<TJoin>(JoinType joinType, ISqlFilter condition, SqlAlias<TJoin> joinAlias)
        {
            ISqlSelect result;
            switch (joinType)
            {
                case JoinType.Inner:
                    result = InnerJoin(condition, joinAlias);
                    break;
                case JoinType.Left:
                    result = LeftJoin(condition, joinAlias);
                    break;
                case JoinType.Right:
                    result = RightJoin(condition, joinAlias);
                    break;
                case JoinType.Full:
                    result = FullJoin(condition, joinAlias);
                    break;
                default:
                    throw new NotSupportedException($"{joinType.ToString()} is not supported");
            }
            return result;
        }

        //-------------------------------------------------------------------------

        ISqlSelect ISqlSelect.Where(ISqlFilter filter) => Where(filter);
        public SqlSelect Where(ISqlFilter filter)
        {
            Guard.IsNotNull(filter);
            return CreateSqlSelect(Info.Where(filter));
        }

        ISqlSelect ISqlSelect.Having(ISqlFilter filter) => Having(filter);
        public SqlSelect Having(ISqlFilter filter)
        {
            Guard.IsNotNull(filter);
            return CreateSqlSelect(Info.Having(filter));
        }
    }
}

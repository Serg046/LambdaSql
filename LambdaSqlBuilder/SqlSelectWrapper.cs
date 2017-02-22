using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using GuardExtensions;
using LambdaSqlBuilder.Field;
using LambdaSqlBuilder.Filter;

namespace LambdaSqlBuilder
{
    public class SqlSelect : SqlSelectBase, ISqlSelect
    {
        private readonly ISqlSelect _innerSqlSelect;
        private readonly ISqlAlias _alias;

        public SqlSelect(ISqlSelect innerSqlSelect, ISqlAlias alias) : base(alias)
        {
            Guard.IsNotNull(innerSqlSelect, alias);
            _innerSqlSelect = innerSqlSelect;
            _alias = alias;
        }

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
                    .Append(SEPARATOR).Append(") AS ").Append(_alias.Value);
                if (GroupByFields.Count > 0)
                {
                    sb.Append(SEPARATOR).Append("GROUP BY")
                        .Append(SEPARATOR_WITH_OFFSET).Append(string.Join(", ", GroupByFields.Select(f => f.ShortView)));
                }
                return sb.ToString();
            }
        }

        private void CheckSelectedFields()
        {
            foreach (var selectField in SelectFields)
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

        ISqlSelect ISqlSelect.Top(int top, bool topByPercent) => Top(top, topByPercent);
        public SqlSelect Top(int top, bool topByPercent = false)
        {
            if (!topByPercent)
            {
                if (top < 1)
                    throw new InvalidOperationException("The value must be greather than 0");
            }
            else
            {
                if (top < 1 || top > 100)
                    throw new InvalidOperationException("The value must be greather between 0 and 100");
            }
            TopByPercent = topByPercent;
            TopLimit = top;
            return this;
        }

        public SqlSelect Distinct()
        {
            Distinct(true);
            return this;
        }

        ISqlSelect ISqlSelect.Distinct(bool isDistinct) => Distinct(isDistinct);
        public SqlSelect Distinct(bool isDistinct)
        {
            IsDistinct = isDistinct;
            return this;
        }

        //-------------------------------------------------------------------------

        ISqlSelect ISqlSelect.AddFields(ISqlField[] fields) => AddFields(fields);
        public SqlSelect AddFields(params ISqlField[] fields)
        {
            Guard.IsNotNull(fields);
            SelectFields.AddRange(fields.Select(f =>
            {
                f.Alias = _alias;
                return f;
            }));
            return this;
        }

        public SqlSelect AddFields<TEntity>(params Expression<Func<TEntity, object>>[] fields)
        {
            Guard.IsNotNull(fields);
            AddFields(_alias, fields, SelectFields);
            return this;
        }

        ISqlSelect ISqlSelect.AddFields<TEntity>(SqlAlias<TEntity> alias, params Expression<Func<TEntity, object>>[] fields)
            => AddFields(fields);

        //-------------------------------------------------------------------------

        ISqlSelect ISqlSelect.GroupBy(ISqlField[] fields) => GroupBy(fields);
        public SqlSelect GroupBy(params ISqlField[] fields)
        {
            Guard.IsNotNull(fields);
            GroupByFields.AddRange(fields.Select(f =>
            {
                f.Alias = _alias;
                return f;
            }));
            return this;
        }

        public SqlSelect GroupBy<TEntity>(params Expression<Func<TEntity, object>>[] fields)
        {
            Guard.IsNotNull(fields);
            AddFields(_alias, fields, GroupByFields);
            return this;
        }

        ISqlSelect ISqlSelect.GroupBy<TEntity>(SqlAlias<TEntity> alias, params Expression<Func<TEntity, object>>[] fields)
            => GroupBy(fields);

        //-------------------------------------------------------------------------

        ISqlSelect ISqlSelect.OrderBy(ISqlField[] fields) => OrderBy(fields);
        public SqlSelect OrderBy(params ISqlField[] fields)
        {
            Guard.IsNotNull(fields);
            OrderByFields.AddRange(fields.Select(f =>
            {
                f.Alias = _alias;
                return f;
            }));
            return this;
        }

        public SqlSelect OrderBy<TEntity>(params Expression<Func<TEntity, object>>[] fields)
        {
            Guard.IsNotNull(fields);
            AddFields(_alias, fields, OrderByFields);
            return this;
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
            Join(JoinType.Inner, condition, joinAlias);
            return this;
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
            Join(JoinType.Left, condition, joinAlias);
            return this;
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
            Join(JoinType.Right, condition, joinAlias);
            return this;
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
            Join(JoinType.Full, condition, joinAlias);
            return this;
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
            WhereFilter = filter;
            return this;
        }

        ISqlSelect ISqlSelect.Having(ISqlFilter filter) => Having(filter);
        public SqlSelect Having(ISqlFilter filter)
        {
            Guard.IsNotNull(filter);
            HavingFilter = filter;
            return this;
        }
    }
}

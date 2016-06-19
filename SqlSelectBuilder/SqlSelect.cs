using System;
using System.Linq.Expressions;
using System.Text;
using GuardExtensions;

namespace SqlSelectBuilder
{
    public class SqlSelect<T> : SqlSelectBase, ISqlSelect
    {
        public SqlSelect() : base(SqlSelectBuilder.MetadataProvider.Instance.AliasFor<T>())
        {
        }

        public string CommandText
        {
            get
            {
                CheckAsAliases();
                var sb = new StringBuilder("SELECT").Append(SEPARATOR_WITH_OFFSET);
                SelectedFields(sb);
                sb.Append("FROM").Append(SEPARATOR_WITH_OFFSET)
                    .Append(MetadataProvider.GetTableName<T>())
                    .Append(" ").Append(MetadataProvider.AliasFor<T>().Value);
                if (Joins.Count > 0)
                {
                    sb.Append(SEPARATOR).Append(string.Join(SEPARATOR, Joins));
                }
                if (WhereFilter != null)
                {
                    sb.Append(SEPARATOR).Append("WHERE")
                        .Append(SEPARATOR_WITH_OFFSET).Append(WhereFilter.Filter);
                }
                if (GroupByFields.Count > 0)
                {
                    sb.Append(SEPARATOR).Append("GROUP BY")
                        .Append(SEPARATOR_WITH_OFFSET).Append(string.Join(", ", GroupByFields));
                }
                if (HavingFilter != null)
                {
                    sb.Append(SEPARATOR).Append("HAVING")
                        .Append(SEPARATOR_WITH_OFFSET).Append(HavingFilter.Filter);
                }
                if (OrderByFields.Count > 0)
                {
                    sb.Append(SEPARATOR).Append("ORDER BY")
                        .Append(SEPARATOR_WITH_OFFSET).Append(string.Join(", ", OrderByFields));
                }
                return sb.ToString();
            }
        }

        public override string ToString()
        {
            return CommandText;
        }

        public Type EntityType => typeof(T);

        ISqlSelect ISqlSelect.Top(int top, bool topByPercent) => Top(top, topByPercent);
        public SqlSelect<T> Top(int top, bool topByPercent = false)
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

        public SqlSelect<T> Distinct()
        {
            Distinct(true);
            return this;
        }

        ISqlSelect ISqlSelect.Distinct(bool isDistinct) => Distinct(isDistinct);
        public SqlSelect<T> Distinct(bool isDistinct)
        {
            IsDistinct = isDistinct;
            return this;
        }

        //-------------------------------------------------------------------------

        ISqlSelect ISqlSelect.AddFields(ISqlField[] fields) => AddFields(fields);
        public SqlSelect<T> AddFields(params ISqlField[] fields)
        {
            Guard.IsNotNull(fields);
            SelectFields.AddRange(fields);
            return this;
        }

        public SqlSelect<T> AddFields(params Expression<Func<T, object>>[] fields)
        {
            Guard.IsNotNull(fields);
            AddFields(MetadataProvider.AliasFor<T>(), fields, SelectFields);
            return this;
        }

        public SqlSelect<T> AddFields<TEntity>(params Expression<Func<TEntity, object>>[] fields)
        {
            Guard.IsNotNull(fields);
            AddFields(MetadataProvider.AliasFor<TEntity>(), fields, SelectFields);
            return this;
        }

        ISqlSelect ISqlSelect.AddFields<TEntity>(SqlAlias<TEntity> alias,
            params Expression<Func<TEntity, object>>[] fields)
        {
            if (alias == null)
                MetadataProvider.AliasFor<TEntity>();
            return AddFields(alias, fields);
        }

        public SqlSelect<T> AddFields<TEntity>(SqlAlias<TEntity> alias,
            params Expression<Func<TEntity, object>>[] fields)
        {
            Guard.IsNotNull(alias);
            Guard.IsNotNull(fields);
            AddFields(alias, fields, SelectFields);
            return this;
        }

        //-------------------------------------------------------------------------

        ISqlSelect ISqlSelect.GroupBy(ISqlField[] fields) => GroupBy(fields);
        public SqlSelect<T> GroupBy(params ISqlField[] fields)
        {
            Guard.IsNotNull(fields);
            GroupByFields.AddRange(fields);
            return this;
        }

        public SqlSelect<T> GroupBy(params Expression<Func<T, object>>[] fields)
        {
            Guard.IsNotNull(fields);
            AddFields(MetadataProvider.AliasFor<T>(), fields, GroupByFields);
            return this;
        }

        public SqlSelect<T> GroupBy<TEntity>(params Expression<Func<TEntity, object>>[] fields)
        {
            Guard.IsNotNull(fields);
            AddFields(MetadataProvider.AliasFor<TEntity>(), fields, GroupByFields);
            return this;
        }

        ISqlSelect ISqlSelect.GroupBy<TEntity>(SqlAlias<TEntity> alias,
            params Expression<Func<TEntity, object>>[] fields)
        {
            if (alias == null)
                MetadataProvider.AliasFor<TEntity>();
            return GroupBy(alias, fields);
        }

        public SqlSelect<T> GroupBy<TEntity>(SqlAlias<TEntity> alias,
            params Expression<Func<TEntity, object>>[] fields)
        {
            Guard.IsNotNull(alias);
            Guard.IsNotNull(fields);
            AddFields(alias, fields, GroupByFields);
            return this;
        }

        //-------------------------------------------------------------------------

        ISqlSelect ISqlSelect.OrderBy(ISqlField[] fields) => OrderBy(fields);
        public SqlSelect<T> OrderBy(params ISqlField[] fields)
        {
            Guard.IsNotNull(fields);
            OrderByFields.AddRange(fields);
            return this;
        }

        public SqlSelect<T> OrderBy(params Expression<Func<T, object>>[] fields)
        {
            Guard.IsNotNull(fields);
            AddFields(MetadataProvider.AliasFor<T>(), fields, OrderByFields);
            return this;
        }

        public SqlSelect<T> OrderBy<TEntity>(params Expression<Func<TEntity, object>>[] fields)
        {
            Guard.IsNotNull(fields);
            AddFields(MetadataProvider.AliasFor<TEntity>(), fields, OrderByFields);
            return this;
        }

        ISqlSelect ISqlSelect.OrderBy<TEntity>(SqlAlias<TEntity> alias,
            params Expression<Func<TEntity, object>>[] fields)
        {
            if (alias == null)
                MetadataProvider.AliasFor<TEntity>();
            return OrderBy(alias, fields);
        }

        public SqlSelect<T> OrderBy<TEntity>(SqlAlias<TEntity> alias,
            params Expression<Func<TEntity, object>>[] fields)
        {
            Guard.IsNotNull(alias);
            Guard.IsNotNull(fields);
            AddFields(alias, fields, OrderByFields);
            return this;
        }

        //-------------------------------------------------------------------------

        public SqlSelect<T> InnerJoin<TLeft, TJoin>(Expression<Func<TLeft, TJoin, bool>> condition,
            SqlAlias<TLeft> leftAlias = null, SqlAlias<TJoin> joinAlias = null)
        {
            Guard.IsNotNull(condition);
            if (leftAlias == null)
                leftAlias = MetadataProvider.AliasFor<TLeft>();
            if (joinAlias == null)
                joinAlias = MetadataProvider.AliasFor<TJoin>();

            return InnerJoin(GetJoinFilter(condition.Body as BinaryExpression, leftAlias, joinAlias), joinAlias);
        }

        public SqlSelect<T> InnerJoin<TJoin>(ISqlFilter condition, SqlAlias<TJoin> joinAlias = null)
        {
            Guard.IsNotNull(condition);
            Join(JoinType.Inner, condition, joinAlias);
            return this;
        }

        public SqlSelect<T> LeftJoin<TLeft, TJoin>(Expression<Func<TLeft, TJoin, bool>> condition,
            SqlAlias<TLeft> leftAlias = null, SqlAlias<TJoin> joinAlias = null)
        {
            Guard.IsNotNull(condition);
            if (leftAlias == null)
                leftAlias = MetadataProvider.AliasFor<TLeft>();
            if (leftAlias == null)
                joinAlias = MetadataProvider.AliasFor<TJoin>();

            return LeftJoin(GetJoinFilter(condition.Body as BinaryExpression, leftAlias, joinAlias), joinAlias);
        }

        public SqlSelect<T> LeftJoin<TJoin>(ISqlFilter condition, SqlAlias<TJoin> joinAlias = null)
        {
            Guard.IsNotNull(condition);
            Join(JoinType.Left, condition, joinAlias);
            return this;
        }

        public SqlSelect<T> RightJoin<TLeft, TJoin>(Expression<Func<TLeft, TJoin, bool>> condition,
            SqlAlias<TLeft> leftAlias = null, SqlAlias<TJoin> joinAlias = null)
        {
            Guard.IsNotNull(condition);
            if (leftAlias == null)
                leftAlias = MetadataProvider.AliasFor<TLeft>();
            if (joinAlias == null)
                joinAlias = MetadataProvider.AliasFor<TJoin>();

            return RightJoin(GetJoinFilter(condition.Body as BinaryExpression, leftAlias, joinAlias), joinAlias);
        }

        public SqlSelect<T> RightJoin<TJoin>(ISqlFilter condition, SqlAlias<TJoin> joinAlias = null)
        {
            Guard.IsNotNull(condition);
            Join(JoinType.Right, condition, joinAlias);
            return this;
        }

        public SqlSelect<T> FullJoin<TLeft, TJoin>(Expression<Func<TLeft, TJoin, bool>> condition,
            SqlAlias<TLeft> leftAlias = null, SqlAlias<TJoin> joinAlias = null)
        {
            Guard.IsNotNull(condition);
            if (leftAlias == null)
                leftAlias = MetadataProvider.AliasFor<TLeft>();
            if (joinAlias == null)
                joinAlias = MetadataProvider.AliasFor<TJoin>();

            return FullJoin(GetJoinFilter(condition.Body as BinaryExpression, leftAlias, joinAlias), joinAlias);
        }

        public SqlSelect<T> FullJoin<TJoin>(ISqlFilter condition, SqlAlias<TJoin> joinAlias = null)
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
        public SqlSelect<T> Where(ISqlFilter filter)
        {
            WhereFilter = filter;
            return this;
        }

        ISqlSelect ISqlSelect.Having(ISqlFilter filter) => Having(filter);
        public SqlSelect<T> Having(ISqlFilter filter)
        {
            HavingFilter = filter;
            return this;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using GuardExtensions;
using SqlSelectBuilder.SqlFilter;

namespace SqlSelectBuilder
{
    public class SqlSelect<T> : SqlSelectBase<T>, ISqlSelect
    {
        // ReSharper disable InconsistentNaming
        private const string SEPARATOR = "\r\n";
        private const string SEPARATOR_WITH_OFFSET = "\r\n    ";
        // ReSharper restore InconsistentNaming


        public string CommandText
        {
            get
            {
                CheckAliases();
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
                        .Append(SEPARATOR_WITH_OFFSET).Append(string.Join(",", GroupByFields));
                }
                if (HavingFilter != null)
                {
                    sb.Append(SEPARATOR).Append("HAVING")
                        .Append(SEPARATOR_WITH_OFFSET).Append(HavingFilter.Filter);
                }
                if (OrderByFields.Count > 0)
                {
                    sb.Append(SEPARATOR).Append("ORDER BY")
                        .Append(SEPARATOR_WITH_OFFSET).Append(string.Join(",", OrderByFields));
                }
                return sb.ToString();
            }
        }

        Type ISqlSelect.EntityType => typeof(T);
        IEnumerable<ISqlField> ISqlSelect.SelectFields => Info.SelectFields;
        IEnumerable<ISqlField> ISqlSelect.GroupByFields => Info.GroupByFields;
        IEnumerable<ISqlField> ISqlSelect.OrderByFields => Info.OrderByFields;
        IEnumerable<ISqlAlias> ISqlSelect.Aliases => Info.Aliases;

        private void CheckAliases()
        {
            // ReSharper disable PossibleMultipleEnumeration
            var allFields = SelectFields.Union(GroupByFields).Union(OrderByFields);
            var intersect = Info.Aliases.Select(a => a.Value).Intersect(allFields.Select(a => a.AsAlias));
            if (intersect.Any())
                throw new InvalidOperationException($"The following user aliases are incorrect: {string.Join(",", intersect)}.");
            // ReSharper restore PossibleMultipleEnumeration
        }

        private void SelectedFields(StringBuilder sb)
        {
            if (IsDistinct)
                sb.Append("DISTINCT ");
            if (TopLimit.HasValue)
            {
                sb.Append($"TOP {TopLimit} ");
                if (TopByPercent)
                    sb.Append("PERCENT ");
            }
            sb.Append(SelectFields.Count == 0 ? "*" : string.Join(",", SelectFields)).Append(SEPARATOR);
        }

        public override string ToString()
        {
            return CommandText;
        }

        void ISqlSelect.Top(int top, bool topByPercent) => Top(top, topByPercent);
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
            Contract.Ensures(Contract.Result<SqlSelect<T>>() != null);
            Contract.EndContractBlock();
            TopByPercent = topByPercent;
            TopLimit = top;
            return this;
        }

        public SqlSelect<T> Distinct()
        {
            Distinct(true);
            return this;
        }

        void ISqlSelect.Distinct(bool isDistinct) => Distinct(isDistinct);
        public SqlSelect<T> Distinct(bool isDistinct)
        {
            IsDistinct = isDistinct;
            return this;
        }

        //-------------------------------------------------------------------------

        void ISqlSelect.AddField(ISqlField field) => AddField(field);
        public SqlSelect<T> AddField(ISqlField field)
        {
            Guard.IsNotNull(field);
            SelectFields.Add(field);
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

        void ISqlSelect.AddFields<TEntity>(SqlAlias<TEntity> alias,
            params Expression<Func<TEntity, object>>[] fields)
        {
            if (alias == null)
                MetadataProvider.AliasFor<TEntity>();
            AddFields(alias, fields);
        }

        public SqlSelect<T> AddFields<TEntity>(SqlAlias<TEntity> alias,
            params Expression<Func<TEntity, object>>[] fields)
        {
            Guard.IsNotNull(alias);
            Guard.IsNotNull(fields);
            AddFields(alias, fields, SelectFields);
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

        void ISqlSelect.GroupBy<TEntity>(SqlAlias<TEntity> alias,
            params Expression<Func<TEntity, object>>[] fields)
        {
            if (alias == null)
                MetadataProvider.AliasFor<TEntity>();
            GroupBy(alias, fields);
        }

        public SqlSelect<T> GroupBy<TEntity>(SqlAlias<TEntity> alias,
            params Expression<Func<TEntity, object>>[] fields)
        {
            Guard.IsNotNull(alias);
            Guard.IsNotNull(fields);
            AddFields(alias, fields, GroupByFields);
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

        void ISqlSelect.OrderBy<TEntity>(SqlAlias<TEntity> alias,
            params Expression<Func<TEntity, object>>[] fields)
        {
            if (alias == null)
                MetadataProvider.AliasFor<TEntity>();
            OrderBy(alias, fields);
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

        void ISqlSelect.Join<TJoin>(JoinType joinType, ISqlFilter condition, SqlAlias<TJoin> joinAlias)
        {
            switch (joinType)
            {
                case JoinType.Inner:
                    InnerJoin(condition, joinAlias);
                    break;
                case JoinType.Left:
                    LeftJoin(condition, joinAlias);
                    break;
                case JoinType.Right:
                    RightJoin(condition, joinAlias);
                    break;
                case JoinType.Full:
                    FullJoin(condition, joinAlias);
                    break;
                default:
                    throw new NotSupportedException($"{joinType.ToString()} is not supported");
            }
        }

        //-------------------------------------------------------------------------

        void ISqlSelect.Where(ISqlFilter filter) => Where(filter);
        public SqlSelect<T> Where(ISqlFilter filter)
        {
            Guard.IsNotNull(filter);
            WhereFilter = filter;
            return this;
        }

        void ISqlSelect.Having(ISqlFilter filter) => Having(filter);
        public SqlSelect<T> Having(ISqlFilter filter)
        {
            Guard.IsNotNull(filter);
            HavingFilter = filter;
            return this;
        }
    }
}

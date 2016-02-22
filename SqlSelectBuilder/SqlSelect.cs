using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GuardExtensions;

namespace SqlSelectBuilder
{
    [ContractClass(typeof(ISqlSelectContract))]
    public interface ISqlSelect
    {
        string CommandText { get; }
        IEnumerable<ISqlField> Fields { get; }
        IEnumerable<ISqlAlias> Aliases { get; }
    }

    [ContractClassFor(typeof(ISqlSelect))]
    [ExcludeFromCodeCoverage]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal abstract class ISqlSelectContract : ISqlSelect
    {
        public IEnumerable<ISqlAlias> Aliases
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<ISqlAlias>>() != null);
                throw new NotImplementedException();
            }
        }

        public string CommandText
        {
            get
            {
                Contract.Ensures(Contract.Result<string>().IsNotEmpty());
                throw new NotImplementedException();
            }
        }

        public IEnumerable<ISqlField> Fields
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<ISqlField>>() != null);
                throw new NotImplementedException();
            }
        }
    }

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

        IEnumerable<ISqlField> ISqlSelect.Fields => Info.SelectFields;
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

        public SqlSelect<T> Distinct(bool isDistinct)
        {
            IsDistinct = isDistinct;
            return this;
        }

        //-------------------------------------------------------------------------

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

        public SqlSelect<T> InnerJoin<TJoin>(SqlFilter<TJoin> condition, SqlAlias<TJoin> joinAlias = null)
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

        public SqlSelect<T> LeftJoin<TJoin>(SqlFilter<TJoin> condition, SqlAlias<TJoin> joinAlias = null)
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

        public SqlSelect<T> RightJoin<TJoin>(SqlFilter<TJoin> condition, SqlAlias<TJoin> joinAlias = null)
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

        public SqlSelect<T> FullJoin<TJoin>(SqlFilter<TJoin> condition, SqlAlias<TJoin> joinAlias = null)
        {
            Guard.IsNotNull(condition);
            Join(JoinType.Full, condition, joinAlias);
            return this;
        }

        //-------------------------------------------------------------------------

        public SqlSelect<T> Where(ISqlFilter filter)
        {
            Guard.IsNotNull(filter);
            WhereFilter = filter;
            return this;
        }

        public SqlSelect<T> Having(ISqlFilter filter)
        {
            Guard.IsNotNull(filter);
            HavingFilter = filter;
            return this;
        }
    }
}

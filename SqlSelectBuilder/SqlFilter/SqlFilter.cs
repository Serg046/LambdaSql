using System;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using GuardExtensions;

namespace SqlSelectBuilder.SqlFilter
{
    public class SqlFilter<TEntity> : SqlFilterBase<TEntity>
    {
        internal SqlFilter(ImmutableList<ISqlFilterItem> sqlFilterItems) : base(sqlFilterItems)
        {
        }

        private SqlFilterField<TEntity, TType> AddFilter<TType>(
            ImmutableList<ISqlFilterItem> items, LambdaExpression field, SqlAlias<TEntity> alias)
        {
            Guard.IsNotNull(field);
            return new SqlFilterField<TEntity, TType>(items, BuildSqlFilterItem(field, alias));
        }

        private SqlFilterField<TEntity, TType> AddFilter<TType>(
            ImmutableList<ISqlFilterItem> items, ISqlField field)
        {
            Guard.IsNotNull(field);
            return new SqlFilterField<TEntity, TType>(items, BuildSqlFilterItem(field));
        }

        public static SqlFilterField<TEntity, TType> From<TType>(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilterField<TEntity, TType>>() != null);
            Guard.IsNotNull(field);
            return new SqlFilterField<TEntity, TType>(ImmutableList<ISqlFilterItem>.Empty, BuildSqlFilterItem(field, alias));
        }

        public static SqlFilterField<TEntity, TType> From<TType>(ISqlField field)
        {
            Contract.Ensures(Contract.Result<SqlFilterField<TEntity, TType>>() != null);
            Guard.IsNotNull(field);
            return new SqlFilterField<TEntity, TType>(ImmutableList<ISqlFilterItem>.Empty, BuildSqlFilterItem(field));
        }

        public SqlFilterField<TEntity, TType> And<TType>(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilterField<TEntity, TType>>() != null);
            Guard.IsNotNull(field);
            return AddFilter<TType>(FilterItems.Add(SqlFilterItems.And), field, alias);
        }

        public SqlFilterField<TEntity, TType> Or<TType>(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilterField<TEntity, TType>>() != null);
            Guard.IsNotNull(field);
            return AddFilter<TType>(FilterItems.Add(SqlFilterItems.Or), field, alias);
        }

        public SqlFilterField<TEntity, TType> And<TType>(ISqlField field)
        {
            Contract.Ensures(Contract.Result<SqlFilterField<TEntity, TType>>() != null);
            Guard.IsNotNull(field);
            return AddFilter<TType>(FilterItems.Add(SqlFilterItems.And), field);
        }

        public SqlFilterField<TEntity, TType> Or<TType>(ISqlField field)
        {
            Contract.Ensures(Contract.Result<SqlFilterField<TEntity, TType>>() != null);
            Guard.IsNotNull(field);
            return AddFilter<TType>(FilterItems.Add(SqlFilterItems.Or), field);
        }

        public SqlFilter<TEntity> And(ISqlFilterItems filter)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(filter);
            var items = FilterItems
                .Add(SqlFilterItems.And)
                .AddRange(filter.FilterItems);
            return new SqlFilter<TEntity>(items);
        }

        public SqlFilter<TEntity> Or(ISqlFilterItems filter)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(filter);
            var items = FilterItems
                .Add(SqlFilterItems.Or)
                .AddRange(filter.FilterItems);
            return new SqlFilter<TEntity>(items);
        }

        public SqlFilter<TEntity> AndGroup(ISqlFilterItems filter)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(filter);
            var items = FilterItems
                .Add(SqlFilterItems.And)
                .Add(SqlFilterItems.Build("("))
                .AddRange(filter.FilterItems)
                .Add(SqlFilterItems.Build(")"));
            return new SqlFilter<TEntity>(items);
        }

        public SqlFilter<TEntity> OrGroup(ISqlFilterItems filter)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(filter);
            var items = FilterItems
                .Add(SqlFilterItems.Or)
                .Add(SqlFilterItems.Build("("))
                .AddRange(filter.FilterItems)
                .Add(SqlFilterItems.Build(")"));
            return new SqlFilter<TEntity>(items);
        }

        public SqlFilter<TEntity> WithoutAliases()
        {
            var filter = new SqlFilter<TEntity>(FilterItems);
            filter.MustBeWithoutAliases = true;
            return filter;
        }

        public SqlFilter<TEntity> WithAliases()
        {
            var filter = new SqlFilter<TEntity>(FilterItems);
            filter.MustBeWithoutAliases = false;
            return filter;
        }
    }
}

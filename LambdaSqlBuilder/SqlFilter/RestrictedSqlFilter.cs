using System;
using System.Collections.Immutable;
using System.Linq.Expressions;
using LambdaSqlBuilder.SqlFilter.SqlFilterField;
using LambdaSqlBuilder.SqlFilter.SqlFilterItem;

namespace LambdaSqlBuilder.SqlFilter
{
    public class RestrictedSqlFilter<TEntity> : SqlFilterBase
    {
        internal RestrictedSqlFilter(ImmutableList<SqlFilterItemFunc> sqlFilterItems) : base(sqlFilterItems)
        {
        }

        private RestrictedSqlFilterField<TEntity, TType, RestrictedSqlFilter<TEntity>> CreateField<TType>(
            ImmutableList<SqlFilterItemFunc> items, LambdaExpression field, SqlAlias<TEntity> alias)
        {
            return new RestrictedSqlFilterField<TEntity, TType, RestrictedSqlFilter<TEntity>>(items, BuildSqlField(field, alias));
        }

        private RestrictedSqlFilterField<TEntity, TType, RestrictedSqlFilter<TEntity>> CreateField<TType>(
            ImmutableList<SqlFilterItemFunc> items, SqlField<TEntity> field)
        {
            return new RestrictedSqlFilterField<TEntity, TType, RestrictedSqlFilter<TEntity>>(items, field);
        }

        public static RestrictedSqlFilterField<TEntity, TType, RestrictedSqlFilter<TEntity>> From<TType>(
            Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            return new RestrictedSqlFilterField<TEntity, TType, RestrictedSqlFilter<TEntity>>(
                ImmutableList<SqlFilterItemFunc>.Empty, BuildSqlField(field, alias));
        }

        public static RestrictedSqlFilterField<TEntity, TType, RestrictedSqlFilter<TEntity>> From<TType>(SqlField<TEntity> field)
        {
            return new RestrictedSqlFilterField<TEntity, TType, RestrictedSqlFilter<TEntity>>(
                ImmutableList<SqlFilterItemFunc>.Empty, field);
        }

        public RestrictedSqlFilterField<TEntity, TType, RestrictedSqlFilter<TEntity>> And<TType>(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            return CreateField<TType>(FilterItems.Add(SqlFilterItems.And), field, alias);
        }

        public RestrictedSqlFilterField<TEntity, TType, RestrictedSqlFilter<TEntity>> Or<TType>(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            return CreateField<TType>(FilterItems.Add(SqlFilterItems.Or), field, alias);
        }

        public RestrictedSqlFilterField<TEntity, TType, RestrictedSqlFilter<TEntity>> And<TType>(SqlField<TEntity> field)
        {
            return CreateField<TType>(FilterItems.Add(SqlFilterItems.And), field);
        }

        public RestrictedSqlFilterField<TEntity, TType, RestrictedSqlFilter<TEntity>> Or<TType>(SqlField<TEntity> field)
        {
            return CreateField<TType>(FilterItems.Add(SqlFilterItems.Or), field);
        }

        public RestrictedSqlFilter<TEntity> And(RestrictedSqlFilter<TEntity> filter)
        {
            var items = FilterItems
                .Add(SqlFilterItems.And)
                .AddRange(filter.FilterItems);
            return new RestrictedSqlFilter<TEntity>(items);
        }

        public RestrictedSqlFilter<TEntity> Or(RestrictedSqlFilter<TEntity> filter)
        {
            var items = FilterItems
                .Add(SqlFilterItems.Or)
                .AddRange(filter.FilterItems);
            return new RestrictedSqlFilter<TEntity>(items);
        }

        public RestrictedSqlFilter<TEntity> AndGroup(RestrictedSqlFilter<TEntity> filter)
        {
            var items = FilterItems
                .Add(SqlFilterItems.And)
                .Add(SqlFilterItems.Build("("))
                .AddRange(filter.FilterItems)
                .Add(SqlFilterItems.Build(")"));
            return new RestrictedSqlFilter<TEntity>(items);
        }

        public RestrictedSqlFilter<TEntity> OrGroup(RestrictedSqlFilter<TEntity> filter)
        {
            var items = FilterItems
                .Add(SqlFilterItems.Or)
                .Add(SqlFilterItems.Build("("))
                .AddRange(filter.FilterItems)
                .Add(SqlFilterItems.Build(")"));
            return new RestrictedSqlFilter<TEntity>(items);
        }

        public RestrictedSqlFilter<TEntity> WithoutAliases()
        {
            var filter = new RestrictedSqlFilter<TEntity>(FilterItems);
            filter.MustBeWithoutAliases = true;
            return filter;
        }

        public RestrictedSqlFilter<TEntity> WithAliases()
        {
            var filter = new RestrictedSqlFilter<TEntity>(FilterItems);
            filter.MustBeWithoutAliases = false;
            return filter;
        }
    }
}

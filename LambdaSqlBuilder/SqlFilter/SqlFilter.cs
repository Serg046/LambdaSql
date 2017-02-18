using System;
using System.Collections.Immutable;
using System.Linq.Expressions;
using LambdaSqlBuilder.SqlFilter.SqlFilterItem;

namespace LambdaSqlBuilder.SqlFilter
{
    public class SqlFilter<TEntity> : SqlFilterBase
    {
        internal SqlFilter(ImmutableList<SqlFilterItemFunc> sqlFilterItems) : base(sqlFilterItems)
        {
        }

        private static SqlFilterField<TEntity, TType, SqlFilter<TEntity>> CreateField<TType>(
            ImmutableList<SqlFilterItemFunc> items, LambdaExpression field, SqlAlias<TEntity> alias)
        {
            return CreateField<TType>(items, BuildSqlField(field, alias));
        }

        private static SqlFilterField<TEntity, TType, SqlFilter<TEntity>> CreateField<TType>(
            ImmutableList<SqlFilterItemFunc> items, ISqlField field)
        {
            return new SqlFilterField<TEntity, TType, SqlFilter<TEntity>>(items, field, i => new SqlFilter<TEntity>(i));
        }

        public static SqlFilterField<TEntity, TType, SqlFilter<TEntity>> From<TType>(
            Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            return CreateField<TType>(ImmutableList<SqlFilterItemFunc>.Empty, BuildSqlField(field, alias));
        }

        public static SqlFilterField<TEntity, TType, SqlFilter<TEntity>> From<TType>(SqlField<TEntity> field)
        {
            return CreateField<TType>(ImmutableList<SqlFilterItemFunc>.Empty, field);
        }

        public SqlFilterField<TEntity, TType, SqlFilter<TEntity>> And<TType>(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            return CreateField<TType>(FilterItems.Add(SqlFilterItems.And), field, alias);
        }

        public SqlFilterField<TEntity, TType, SqlFilter<TEntity>> Or<TType>(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            return CreateField<TType>(FilterItems.Add(SqlFilterItems.Or), field, alias);
        }

        public SqlFilterField<TEntity, TType, SqlFilter<TEntity>> And<TType>(SqlField<TEntity> field)
        {
            return CreateField<TType>(FilterItems.Add(SqlFilterItems.And), field);
        }

        public SqlFilterField<TEntity, TType, SqlFilter<TEntity>> Or<TType>(SqlField<TEntity> field)
        {
            return CreateField<TType>(FilterItems.Add(SqlFilterItems.Or), field);
        }

        //-----------------------------------------------------------------------------------------------------

        public SqlFilter<TEntity> And(SqlFilter<TEntity> filter)
        {
            var items = FilterItems
                .Add(SqlFilterItems.And)
                .AddRange(filter.FilterItems);
            return new SqlFilter<TEntity>(items);
        }

        public SqlFilter<TEntity> Or(SqlFilter<TEntity> filter)
        {
            var items = FilterItems
                .Add(SqlFilterItems.Or)
                .AddRange(filter.FilterItems);
            return new SqlFilter<TEntity>(items);
        }

        public SqlFilter<TEntity> AndGroup(SqlFilter<TEntity> filter)
        {
            var items = FilterItems
                .Add(SqlFilterItems.And)
                .Add(SqlFilterItems.Build("("))
                .AddRange(filter.FilterItems)
                .Add(SqlFilterItems.Build(")"));
            return new SqlFilter<TEntity>(items);
        }

        public SqlFilter<TEntity> OrGroup(SqlFilter<TEntity> filter)
        {
            var items = FilterItems
                .Add(SqlFilterItems.Or)
                .Add(SqlFilterItems.Build("("))
                .AddRange(filter.FilterItems)
                .Add(SqlFilterItems.Build(")"));
            return new SqlFilter<TEntity>(items);
        }

        //-----------------------------------------------------------------------------------------------------

        public SqlFilterEx<TEntity> And(SqlFilterBase filter)
        {
            var items = FilterItems
                .Add(SqlFilterItems.And)
                .AddRange(filter.FilterItems);
            return new SqlFilterEx<TEntity>(items);
        }

        public SqlFilterEx<TEntity> Or(SqlFilterBase filter)
        {
            var items = FilterItems
                .Add(SqlFilterItems.Or)
                .AddRange(filter.FilterItems);
            return new SqlFilterEx<TEntity>(items);
        }

        public SqlFilterEx<TEntity> AndGroup(SqlFilterBase filter)
        {
            var items = FilterItems
                .Add(SqlFilterItems.And)
                .Add(SqlFilterItems.Build("("))
                .AddRange(filter.FilterItems)
                .Add(SqlFilterItems.Build(")"));
            return new SqlFilterEx<TEntity>(items);
        }

        public SqlFilterEx<TEntity> OrGroup(SqlFilterBase filter)
        {
            var items = FilterItems
                .Add(SqlFilterItems.Or)
                .Add(SqlFilterItems.Build("("))
                .AddRange(filter.FilterItems)
                .Add(SqlFilterItems.Build(")"));
            return new SqlFilterEx<TEntity>(items);
        }

        //-----------------------------------------------------------------------------------------------------

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

        //-----------------------------------------------------------------------------------------------------

        public SqlFilter<TEntity> WithParameterPrefix(string prefix)
        {
            var filter = new SqlFilter<TEntity>(FilterItems);
            filter.ParamPrefix = prefix;
            return filter;
        }
    }
}

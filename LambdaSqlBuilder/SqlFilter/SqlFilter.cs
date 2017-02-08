using System;
using System.Collections.Immutable;
using System.Linq.Expressions;
using LambdaSqlBuilder.SqlFilter.SqlFilterField;
using LambdaSqlBuilder.SqlFilter.SqlFilterItem;

namespace LambdaSqlBuilder.SqlFilter
{
    public class SqlFilter<TEntity> : SqlFilterBase
    {
        internal SqlFilter(ImmutableList<SqlFilterItemFunc> sqlFilterItems) : base(sqlFilterItems)
        {
        }

        private SqlFilterField<TEntity, TType> CreateField<TType>(
            ImmutableList<SqlFilterItemFunc> items, LambdaExpression field, SqlAlias<TEntity> alias)
        {
            return new SqlFilterField<TEntity, TType>(items, BuildSqlField(field, alias));
        }

        private SqlFilterField<TEntity, TType> CreateField<TType>(
            ImmutableList<SqlFilterItemFunc> items, ISqlField field)
        {
            return new SqlFilterField<TEntity, TType>(items, field);
        }

        public static SqlFilterField<TEntity, TType> From<TType>(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            return new SqlFilterField<TEntity, TType>(ImmutableList<SqlFilterItemFunc>.Empty, BuildSqlField(field, alias));
        }

        public static SqlFilterField<TEntity, TType> From<TType>(ISqlField field)
        {
            return new SqlFilterField<TEntity, TType>(ImmutableList<SqlFilterItemFunc>.Empty, field);
        }

        public SqlFilterField<TEntity, TType> And<TType>(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            return CreateField<TType>(FilterItems.Add(SqlFilterItems.And), field, alias);
        }

        public SqlFilterField<TEntity, TType> Or<TType>(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            return CreateField<TType>(FilterItems.Add(SqlFilterItems.Or), field, alias);
        }

        public SqlFilterField<TEntity, TType> And<TType>(ISqlField field)
        {
            return CreateField<TType>(FilterItems.Add(SqlFilterItems.And), field);
        }

        public SqlFilterField<TEntity, TType> Or<TType>(ISqlField field)
        {
            return CreateField<TType>(FilterItems.Add(SqlFilterItems.Or), field);
        }

        public SqlFilter<TEntity> And(SqlFilterBase filter)
        {
            var items = FilterItems
                .Add(SqlFilterItems.And)
                .AddRange(filter.FilterItems);
            return new SqlFilter<TEntity>(items);
        }

        public SqlFilter<TEntity> Or(SqlFilterBase filter)
        {
            var items = FilterItems
                .Add(SqlFilterItems.Or)
                .AddRange(filter.FilterItems);
            return new SqlFilter<TEntity>(items);
        }

        public SqlFilter<TEntity> AndGroup(SqlFilterBase filter)
        {
            var items = FilterItems
                .Add(SqlFilterItems.And)
                .Add(SqlFilterItems.Build("("))
                .AddRange(filter.FilterItems)
                .Add(SqlFilterItems.Build(")"));
            return new SqlFilter<TEntity>(items);
        }

        public SqlFilter<TEntity> OrGroup(SqlFilterBase filter)
        {
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

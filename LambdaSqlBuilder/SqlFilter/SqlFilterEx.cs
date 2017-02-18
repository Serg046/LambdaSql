using System;
using System.Collections.Immutable;
using System.Linq.Expressions;
using LambdaSqlBuilder.SqlFilter.SqlFilterItem;

namespace LambdaSqlBuilder.SqlFilter
{
    public class SqlFilterEx<TEntity> : SqlFilterBase
    {
        internal SqlFilterEx(ImmutableList<SqlFilterItemFunc> sqlFilterItems) : base(sqlFilterItems)
        {
        }

        private static SqlFilterField<TEntity, TType, SqlFilterEx<TEntity>> CreateField<TType>(
            ImmutableList<SqlFilterItemFunc> items, LambdaExpression field, SqlAlias<TEntity> alias)
        {
            return CreateField<TType>(items, BuildSqlField(field, alias));
        }

        private static SqlFilterField<TEntity, TType, SqlFilterEx<TEntity>> CreateField<TType>(
            ImmutableList<SqlFilterItemFunc> items, ISqlField field)
        {
            return new SqlFilterField<TEntity, TType, SqlFilterEx<TEntity>>(items, field, i => new SqlFilterEx<TEntity>(i));
        }

        public SqlFilterField<TEntity, TType, SqlFilterEx<TEntity>> And<TType>(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            return CreateField<TType>(FilterItems.Add(SqlFilterItems.And), field, alias);
        }

        public SqlFilterField<TEntity, TType, SqlFilterEx<TEntity>> Or<TType>(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            return CreateField<TType>(FilterItems.Add(SqlFilterItems.Or), field, alias);
        }

        public SqlFilterField<TEntity, TType, SqlFilterEx<TEntity>> And<TType>(ISqlField field)
        {
            return CreateField<TType>(FilterItems.Add(SqlFilterItems.And), field);
        }

        public SqlFilterField<TEntity, TType, SqlFilterEx<TEntity>> Or<TType>(ISqlField field)
        {
            return CreateField<TType>(FilterItems.Add(SqlFilterItems.Or), field);
        }

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

        public SqlFilterEx<TEntity> WithoutAliases()
        {
            var filter = new SqlFilterEx<TEntity>(FilterItems);
            filter.MustBeWithoutAliases = true;
            return filter;
        }

        public SqlFilterEx<TEntity> WithAliases()
        {
            var filter = new SqlFilterEx<TEntity>(FilterItems);
            filter.MustBeWithoutAliases = false;
            return filter;
        }

        public SqlFilterEx<TEntity> WithParameterPrefix(string prefix)
        {
            var filter = new SqlFilterEx<TEntity>(FilterItems);
            filter.ParamPrefix = prefix;
            return filter;
        }
    }
}

using System;
using System.Collections.Immutable;
using System.Linq.Expressions;
using LambdaSqlBuilder.Field;
using LambdaSqlBuilder.Filter.SqlFilterItem;

namespace LambdaSqlBuilder.Filter
{
    public class SqlFilterEx<TEntity> : SqlFilterBase
    {
        internal SqlFilterEx(ImmutableList<SqlFilterItemFunc> sqlFilterItems) : base(sqlFilterItems)
        {
        }

        private static SqlFilterField<TEntity, TFieldType, SqlFilterEx<TEntity>> CreateField<TFieldType>(
            ImmutableList<SqlFilterItemFunc> items, LambdaExpression field, SqlAlias<TEntity> alias)
        {
            return CreateField<TFieldType>(items, BuildSqlField<TEntity, TFieldType>(field, alias));
        }

        private static SqlFilterField<TEntity, TFieldType, SqlFilterEx<TEntity>> CreateField<TFieldType>(
            ImmutableList<SqlFilterItemFunc> items, ITypedSqlField field)
        {
            return new SqlFilterField<TEntity, TFieldType, SqlFilterEx<TEntity>>(items, field, i => new SqlFilterEx<TEntity>(i));
        }

        //-----------------------------------------------------------------------------------------------------

        public SqlFilterField<TEntity, TFieldType, SqlFilterEx<TEntity>> And<TFieldType>(Expression<Func<TEntity, TFieldType>> field, SqlAlias<TEntity> alias = null)
        {
            return CreateField<TFieldType>(FilterItems.Add(SqlFilterItems.And), field, alias);
        }

        public SqlFilterField<TEntity, TFieldType, SqlFilterEx<TEntity>> And<TFieldType>(SqlField<TEntity, TFieldType> field)
        {
            return CreateField<TFieldType>(FilterItems.Add(SqlFilterItems.And), field);
        }

        public SqlFilterField<TEntity, TFieldType, SqlFilterEx<TEntity>> And<TFieldType>(ITypedSqlField field)
        {
            CheckField<TEntity, TFieldType>(field);
            return CreateField<TFieldType>(FilterItems.Add(SqlFilterItems.And), field);
        }

        public SqlFilterEx<TEntity> And(SqlFilterBase filter)
        {
            var items = FilterItems
                .Add(SqlFilterItems.And)
                .AddRange(filter.FilterItems);
            return new SqlFilterEx<TEntity>(items);
        }

        //-----------------------------------------------------------------------------------------------------

        public SqlFilterField<TEntity, TFieldType, SqlFilterEx<TEntity>> Or<TFieldType>(Expression<Func<TEntity, TFieldType>> field, SqlAlias<TEntity> alias = null)
        {
            return CreateField<TFieldType>(FilterItems.Add(SqlFilterItems.Or), field, alias);
        }

        public SqlFilterField<TEntity, TFieldType, SqlFilterEx<TEntity>> Or<TFieldType>(SqlField<TEntity, TFieldType> field)
        {
            return CreateField<TFieldType>(FilterItems.Add(SqlFilterItems.Or), field);
        }

        public SqlFilterField<TEntity, TFieldType, SqlFilterEx<TEntity>> Or<TFieldType>(ITypedSqlField field)
        {
            CheckField<TEntity, TFieldType>(field);
            return CreateField<TFieldType>(FilterItems.Add(SqlFilterItems.Or), field);
        }

        public SqlFilterEx<TEntity> Or(SqlFilterBase filter)
        {
            var items = FilterItems
                .Add(SqlFilterItems.Or)
                .AddRange(filter.FilterItems);
            return new SqlFilterEx<TEntity>(items);
        }

        //-----------------------------------------------------------------------------------------------------

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

        //-----------------------------------------------------------------------------------------------------

        public SqlFilterEx<TEntity> WithParameterPrefix(string prefix)
        {
            var filter = new SqlFilterEx<TEntity>(FilterItems);
            filter.ParamPrefix = prefix;
            return filter;
        }
    }
}

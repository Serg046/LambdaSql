using System;
using System.Collections.Immutable;
using System.Linq.Expressions;
using LambdaSqlBuilder.Field;
using LambdaSqlBuilder.Filter.SqlFilterItem;

namespace LambdaSqlBuilder.Filter
{
    public class MultitableSqlFilter<TEntity> : SqlFilterBase
    {
        internal MultitableSqlFilter(ImmutableList<SqlFilterItemFunc> sqlFilterItems) : base(sqlFilterItems)
        {
        }

        private static SqlFilterField<TEntity, TFieldType, MultitableSqlFilter<TEntity>> CreateField<TFieldType>(
            ImmutableList<SqlFilterItemFunc> items, LambdaExpression field, SqlAlias<TEntity> alias)
        {
            return CreateField<TFieldType>(items, BuildSqlField<TEntity, TFieldType>(field, alias));
        }

        private static SqlFilterField<TEntity, TFieldType, MultitableSqlFilter<TEntity>> CreateField<TFieldType>(
            ImmutableList<SqlFilterItemFunc> items, ITypedSqlField field)
        {
            return new SqlFilterField<TEntity, TFieldType, MultitableSqlFilter<TEntity>>(items, field, i => new MultitableSqlFilter<TEntity>(i));
        }

        //-----------------------------------------------------------------------------------------------------

        public SqlFilterField<TEntity, TFieldType, MultitableSqlFilter<TEntity>> And<TFieldType>(Expression<Func<TEntity, TFieldType>> field, SqlAlias<TEntity> alias = null)
        {
            return CreateField<TFieldType>(FilterItems.Add(SqlFilterItems.And), field, alias);
        }

        public SqlFilterField<TEntity, TFieldType, MultitableSqlFilter<TEntity>> And<TFieldType>(SqlField<TEntity, TFieldType> field)
        {
            return CreateField<TFieldType>(FilterItems.Add(SqlFilterItems.And), field);
        }

        public SqlFilterField<TEntity, TFieldType, MultitableSqlFilter<TEntity>> And<TFieldType>(ITypedSqlField field)
        {
            CheckField<TEntity, TFieldType>(field);
            return CreateField<TFieldType>(FilterItems.Add(SqlFilterItems.And), field);
        }

        public MultitableSqlFilter<TEntity> And(SqlFilterBase filter)
        {
            var items = FilterItems
                .Add(SqlFilterItems.And)
                .AddRange(filter.FilterItems);
            return new MultitableSqlFilter<TEntity>(items);
        }

        //-----------------------------------------------------------------------------------------------------

        public SqlFilterField<TEntity, TFieldType, MultitableSqlFilter<TEntity>> Or<TFieldType>(Expression<Func<TEntity, TFieldType>> field, SqlAlias<TEntity> alias = null)
        {
            return CreateField<TFieldType>(FilterItems.Add(SqlFilterItems.Or), field, alias);
        }

        public SqlFilterField<TEntity, TFieldType, MultitableSqlFilter<TEntity>> Or<TFieldType>(SqlField<TEntity, TFieldType> field)
        {
            return CreateField<TFieldType>(FilterItems.Add(SqlFilterItems.Or), field);
        }

        public SqlFilterField<TEntity, TFieldType, MultitableSqlFilter<TEntity>> Or<TFieldType>(ITypedSqlField field)
        {
            CheckField<TEntity, TFieldType>(field);
            return CreateField<TFieldType>(FilterItems.Add(SqlFilterItems.Or), field);
        }

        public MultitableSqlFilter<TEntity> Or(SqlFilterBase filter)
        {
            var items = FilterItems
                .Add(SqlFilterItems.Or)
                .AddRange(filter.FilterItems);
            return new MultitableSqlFilter<TEntity>(items);
        }

        //-----------------------------------------------------------------------------------------------------

        public MultitableSqlFilter<TEntity> AndGroup(SqlFilterBase filter)
        {
            var items = FilterItems
                .Add(SqlFilterItems.And)
                .Add(SqlFilterItems.Build("("))
                .AddRange(filter.FilterItems)
                .Add(SqlFilterItems.Build(")"));
            return new MultitableSqlFilter<TEntity>(items);
        }

        public MultitableSqlFilter<TEntity> OrGroup(SqlFilterBase filter)
        {
            var items = FilterItems
                .Add(SqlFilterItems.Or)
                .Add(SqlFilterItems.Build("("))
                .AddRange(filter.FilterItems)
                .Add(SqlFilterItems.Build(")"));
            return new MultitableSqlFilter<TEntity>(items);
        }

        //-----------------------------------------------------------------------------------------------------

        public MultitableSqlFilter<TEntity> WithoutAliases()
        {
            var filter = new MultitableSqlFilter<TEntity>(FilterItems);
            filter.MustBeWithoutAliases = true;
            return filter;
        }

        public MultitableSqlFilter<TEntity> WithAliases()
        {
            var filter = new MultitableSqlFilter<TEntity>(FilterItems);
            filter.MustBeWithoutAliases = false;
            return filter;
        }

        //-----------------------------------------------------------------------------------------------------

        public MultitableSqlFilter<TEntity> WithParameterPrefix(string prefix)
        {
            var filter = new MultitableSqlFilter<TEntity>(FilterItems);
            filter.ParamPrefix = prefix;
            return filter;
        }
    }
}

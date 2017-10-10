using System;
using System.Collections.Immutable;
using System.Linq.Expressions;
using LambdaSql.Field;
using LambdaSql.Filter.SqlFilterItem;

namespace LambdaSql.Filter
{
    public class MultitableSqlFilter<TEntity> : SqlFilterBase
    {
        internal MultitableSqlFilter(ImmutableList<SqlFilterItemCallback> sqlFilterItems) : base(sqlFilterItems)
        {
        }

        private static SqlFilterField<TEntity, TFieldType, MultitableSqlFilter<TEntity>> CreateField<TFieldType>(
            ImmutableList<SqlFilterItemCallback> items, LambdaExpression field, SqlAlias<TEntity> alias)
        {
            return CreateField<TFieldType>(items, BuildSqlField<TEntity, TFieldType>(field, alias));
        }

        private static SqlFilterField<TEntity, TFieldType, MultitableSqlFilter<TEntity>> CreateField<TFieldType>(
            ImmutableList<SqlFilterItemCallback> items, ITypedSqlField field)
        {
            return new SqlFilterField<TEntity, TFieldType, MultitableSqlFilter<TEntity>>(items, field, i => new MultitableSqlFilter<TEntity>(i));
        }

        //-----------------------------------------------------------------------------------------------------

        public SqlFilterField<TEntity, TFieldType, MultitableSqlFilter<TEntity>> And<TFieldType>(Expression<Func<TEntity, TFieldType>> field, SqlAlias<TEntity> alias = null)
        {
            return CreateField<TFieldType>(AddItem(SqlFilterItems.And), field, alias);
        }

        public SqlFilterField<TEntity, TFieldType, MultitableSqlFilter<TEntity>> And<TFieldType>(SqlField<TEntity, TFieldType> field)
        {
            return CreateField<TFieldType>(AddItem(SqlFilterItems.And), field);
        }

        public SqlFilterField<TEntity, TFieldType, MultitableSqlFilter<TEntity>> And<TFieldType>(ITypedSqlField field)
        {
            CheckField<TEntity, TFieldType>(field);
            return CreateField<TFieldType>(AddItem(SqlFilterItems.And), field);
        }

        public SqlFilterField<TEntity, object, MultitableSqlFilter<TEntity>> And(ITypedSqlField field)
        {
            return And<object>(field);
        }

        public MultitableSqlFilter<TEntity> And(SqlFilterBase filter)
        {
            return new MultitableSqlFilter<TEntity>(AddItem(SqlFilterItems.And).AddRange(filter.FilterItems));
        }

        //-----------------------------------------------------------------------------------------------------

        public SqlFilterField<TEntity, TFieldType, MultitableSqlFilter<TEntity>> Or<TFieldType>(Expression<Func<TEntity, TFieldType>> field, SqlAlias<TEntity> alias = null)
        {
            return CreateField<TFieldType>(AddItem(SqlFilterItems.Or), field, alias);
        }

        public SqlFilterField<TEntity, TFieldType, MultitableSqlFilter<TEntity>> Or<TFieldType>(SqlField<TEntity, TFieldType> field)
        {
            return CreateField<TFieldType>(AddItem(SqlFilterItems.Or), field);
        }

        public SqlFilterField<TEntity, TFieldType, MultitableSqlFilter<TEntity>> Or<TFieldType>(ITypedSqlField field)
        {
            CheckField<TEntity, TFieldType>(field);
            return CreateField<TFieldType>(AddItem(SqlFilterItems.Or), field);
        }

        public SqlFilterField<TEntity, object, MultitableSqlFilter<TEntity>> Or(ITypedSqlField field)
        {
            return Or<object>(field);
        }

        public MultitableSqlFilter<TEntity> Or(SqlFilterBase filter)
        {
            return new MultitableSqlFilter<TEntity>(AddItem(SqlFilterItems.Or).AddRange(filter.FilterItems));
        }

        //-----------------------------------------------------------------------------------------------------

        public MultitableSqlFilter<TEntity> AndGroup(SqlFilterBase filter)
        {
            var items = AddItem(SqlFilterItems.And)
                .Add(SqlFilterItems.Build("("))
                .AddRange(filter.FilterItems)
                .Add(SqlFilterItems.Build(")"));
            return new MultitableSqlFilter<TEntity>(items);
        }

        public MultitableSqlFilter<TEntity> OrGroup(SqlFilterBase filter)
        {
            var items = AddItem(SqlFilterItems.Or)
                .Add(SqlFilterItems.Build("("))
                .AddRange(filter.FilterItems)
                .Add(SqlFilterItems.Build(")"));
            return new MultitableSqlFilter<TEntity>(items);
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

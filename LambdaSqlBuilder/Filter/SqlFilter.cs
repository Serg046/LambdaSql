using System;
using System.Collections.Immutable;
using System.Linq.Expressions;
using LambdaSqlBuilder.Field;
using LambdaSqlBuilder.Filter.SqlFilterItem;

namespace LambdaSqlBuilder.Filter
{
    public class SqlFilter<TEntity> : SqlFilterBase
    {
        internal SqlFilter(ImmutableList<SqlFilterItemFunc> sqlFilterItems) : base(sqlFilterItems)
        {
        }

        public static SqlFilter<TEntity> Empty { get; } = new SqlFilter<TEntity>(ImmutableList<SqlFilterItemFunc>.Empty);

        //-----------------------------------------------------------------------------------------------------

        private static SqlFilterField<TEntity, TFieldType, SqlFilter<TEntity>> CreateField<TFieldType>(
            ImmutableList<SqlFilterItemFunc> items, LambdaExpression field, SqlAlias<TEntity> alias)
        {
            return CreateField<TFieldType>(items, BuildSqlField<TEntity, TFieldType>(field, alias));
        }

        private static SqlFilterField<TEntity, TFieldType, SqlFilter<TEntity>> CreateField<TFieldType>(
            ImmutableList<SqlFilterItemFunc> items, ITypedSqlField field)
        {
            return new SqlFilterField<TEntity, TFieldType, SqlFilter<TEntity>>(items, field, i => new SqlFilter<TEntity>(i));
        }

        private ImmutableList<SqlFilterItemFunc> AddItem(SqlFilterItemFunc item)
            => FilterItems.Count == 0 ? FilterItems : FilterItems.Add(item);

        //-----------------------------------------------------------------------------------------------------

        public static SqlFilterField<TEntity, TFieldType, SqlFilter<TEntity>> From<TFieldType>(
            Expression<Func<TEntity, TFieldType>> field, SqlAlias<TEntity> alias = null)
        {
            return CreateField<TFieldType>(ImmutableList<SqlFilterItemFunc>.Empty, BuildSqlField<TEntity, TFieldType>(field, alias));
        }

        public static SqlFilterField<TEntity, TFieldType, SqlFilter<TEntity>> From<TFieldType>(SqlField<TEntity, TFieldType> field)
        {
            return CreateField<TFieldType>(ImmutableList<SqlFilterItemFunc>.Empty, field);
        }

        public static SqlFilterField<TEntity, TFieldType, SqlFilter<TEntity>> From<TFieldType>(ITypedSqlField field)
        {
            CheckField<TEntity, TFieldType>(field);
            return CreateField<TFieldType>(ImmutableList<SqlFilterItemFunc>.Empty, field);
        }

        public static SqlFilterField<TEntity, object, SqlFilter<TEntity>> From(ITypedSqlField field)
        {
            return From<object>(field);
        }

        //-----------------------------------------------------------------------------------------------------

        public SqlFilterField<TEntity, TFieldType, SqlFilter<TEntity>> And<TFieldType>(Expression<Func<TEntity, TFieldType>> field, SqlAlias<TEntity> alias = null)
        {
            return CreateField<TFieldType>(AddItem(SqlFilterItems.And), field, alias);
        }

        public SqlFilterField<TEntity, TFieldType, SqlFilter<TEntity>> And<TFieldType>(SqlField<TEntity, TFieldType> field)
        {
            return CreateField<TFieldType>(AddItem(SqlFilterItems.And), field);
        }

        public SqlFilterField<TEntity, TFieldType, SqlFilter<TEntity>> And<TFieldType>(ITypedSqlField field)
        {
            CheckField<TEntity, TFieldType>(field);
            return CreateField<TFieldType>(AddItem(SqlFilterItems.And), field);
        }

        public SqlFilterField<TEntity, object, SqlFilter<TEntity>> And(ITypedSqlField field)
        {
            return And<object>(field);
        }

        public SqlFilter<TEntity> And(SqlFilter<TEntity> filter)
        {
            return new SqlFilter<TEntity>(AddItem(SqlFilterItems.And).AddRange(filter.FilterItems));
        }

        public MultitableSqlFilter<TEntity> And(SqlFilterBase filter)
        {
            return new MultitableSqlFilter<TEntity>(AddItem(SqlFilterItems.And).AddRange(filter.FilterItems));
        }

        //-----------------------------------------------------------------------------------------------------

        public SqlFilterField<TEntity, TFieldType, SqlFilter<TEntity>> Or<TFieldType>(Expression<Func<TEntity, TFieldType>> field, SqlAlias<TEntity> alias = null)
        {
            return CreateField<TFieldType>(AddItem(SqlFilterItems.Or), field, alias);
        }

        public SqlFilterField<TEntity, TFieldType, SqlFilter<TEntity>> Or<TFieldType>(SqlField<TEntity, TFieldType> field)
        {
            return CreateField<TFieldType>(AddItem(SqlFilterItems.Or), field);
        }

        public SqlFilterField<TEntity, TFieldType, SqlFilter<TEntity>> Or<TFieldType>(ITypedSqlField field)
        {
            CheckField<TEntity, TFieldType>(field);
            return CreateField<TFieldType>(AddItem(SqlFilterItems.Or), field);
        }

        public SqlFilterField<TEntity, object, SqlFilter<TEntity>> Or(ITypedSqlField field)
        {
            return Or<object>(field);
        }

        public SqlFilter<TEntity> Or(SqlFilter<TEntity> filter)
        {
            return new SqlFilter<TEntity>(AddItem(SqlFilterItems.Or).AddRange(filter.FilterItems));
        }

        public MultitableSqlFilter<TEntity> Or(SqlFilterBase filter)
        {
            return new MultitableSqlFilter<TEntity>(AddItem(SqlFilterItems.Or).AddRange(filter.FilterItems));
        }

        //-----------------------------------------------------------------------------------------------------

        public SqlFilter<TEntity> AndGroup(SqlFilter<TEntity> filter)
        {
            var items = AddItem(SqlFilterItems.And)
                .Add(SqlFilterItems.Build("("))
                .AddRange(filter.FilterItems)
                .Add(SqlFilterItems.Build(")"));
            return new SqlFilter<TEntity>(items);
        }

        public MultitableSqlFilter<TEntity> AndGroup(SqlFilterBase filter)
        {
            var items = AddItem(SqlFilterItems.And)
                .Add(SqlFilterItems.Build("("))
                .AddRange(filter.FilterItems)
                .Add(SqlFilterItems.Build(")"));
            return new MultitableSqlFilter<TEntity>(items);
        }

        //-----------------------------------------------------------------------------------------------------

        public SqlFilter<TEntity> OrGroup(SqlFilter<TEntity> filter)
        {
            var items = AddItem(SqlFilterItems.Or)
                .Add(SqlFilterItems.Build("("))
                .AddRange(filter.FilterItems)
                .Add(SqlFilterItems.Build(")"));
            return new SqlFilter<TEntity>(items);
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

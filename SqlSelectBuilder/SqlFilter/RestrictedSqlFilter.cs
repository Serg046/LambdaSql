using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using GuardExtensions;

namespace SqlSelectBuilder.SqlFilter
{
    public class RestrictedSqlFilter<TEntity> : SqlFilterBase<TEntity>
    {
        internal RestrictedSqlFilter()
        {
        }

        internal RestrictedSqlFilter(string filter) : base(filter)
        {
        }

        public override string ToString() => Filter;

        private RestrictedSqlFilterField<TEntity, TType, RestrictedSqlFilter<TEntity>> AddFilter<TType>(string field, string logicSeparator)
        {
            Contract.Requires(field.IsNotEmpty());
            Contract.Requires(logicSeparator.IsNotEmpty());

            return new RestrictedSqlFilterField<TEntity, TType, RestrictedSqlFilter<TEntity>>(Filter + logicSeparator, field);
        }

        public static RestrictedSqlFilterField<TEntity, TType, RestrictedSqlFilter<TEntity>> From<TType>(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilterField<TEntity, TType>>() != null);
            Guard.IsNotNull(field);
            alias = CheckAlias(alias);
            return new RestrictedSqlFilterField<TEntity, TType, RestrictedSqlFilter<TEntity>>(GetFieldName(field, alias));
        }

        public static RestrictedSqlFilterField<TEntity, TType, RestrictedSqlFilter<TEntity>> From<TType>(SqlField<TEntity> field)
        {
            Contract.Ensures(Contract.Result<SqlFilterField<TEntity, TType>>() != null);
            Guard.IsNotNull(field);
            return new RestrictedSqlFilterField<TEntity, TType, RestrictedSqlFilter<TEntity>>(field.ShortView);
        }

        public RestrictedSqlFilterField<TEntity, TType, RestrictedSqlFilter<TEntity>> And<TType>(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilterField<TEntity, TType>>() != null);
            Guard.IsNotNull(field);
            alias = CheckAlias(alias);
            return AddFilter<TType>(GetFieldName(field, alias), AND);
        }

        public RestrictedSqlFilterField<TEntity, TType, RestrictedSqlFilter<TEntity>> Or<TType>(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilterField<TEntity, TType>>() != null);
            Guard.IsNotNull(field);
            alias = CheckAlias(alias);
            return AddFilter<TType>(GetFieldName(field, alias), OR);
        }

        public RestrictedSqlFilterField<TEntity, TType, RestrictedSqlFilter<TEntity>> And<TType>(SqlField<TEntity> field)
        {
            Contract.Ensures(Contract.Result<SqlFilterField<TEntity, TType>>() != null);
            Guard.IsNotNull(field);
            return AddFilter<TType>(field.ShortView, AND);
        }

        public RestrictedSqlFilterField<TEntity, TType, RestrictedSqlFilter<TEntity>> Or<TType>(SqlField<TEntity> field)
        {
            Contract.Ensures(Contract.Result<SqlFilterField<TEntity, TType>>() != null);
            Guard.IsNotNull(field);
            return AddFilter<TType>(field.ShortView, OR);
        }

        public RestrictedSqlFilter<TEntity> AndGroup(RestrictedSqlFilter<TEntity> filter)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(filter);
            return new RestrictedSqlFilter<TEntity>($"{Filter}{AND}({filter.Filter})");
        }

        public RestrictedSqlFilter<TEntity> OrGroup(RestrictedSqlFilter<TEntity> filter)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(filter);
            return new RestrictedSqlFilter<TEntity>($"{Filter}{OR}({filter.Filter})");
        }
    }
}

using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using GuardExtensions;

namespace SqlSelectBuilder.SqlFilter
{
    public class SqlFilter<TEntity> : SqlFilterBase<TEntity>
    {
        internal SqlFilter()
        {
        }

        internal SqlFilter(string filter) : base(filter)
        {
        }

        public override string ToString() => Filter;

        private SqlFilterField<TEntity, TType> AddFilter<TType>(string field, string logicSeparator)
        {
            Contract.Requires(field.IsNotEmpty());
            Contract.Requires(logicSeparator.IsNotEmpty());

            return new SqlFilterField<TEntity, TType>(Filter + logicSeparator, field);
        }

        public static SqlFilterField<TEntity, TType> From<TType>(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilterField<TEntity, TType>>() != null);
            Guard.IsNotNull(field);
            alias = CheckAlias(alias);
            return new SqlFilterField<TEntity, TType>(GetFieldName(field, alias));
        }

        public static SqlFilterField<TEntity, TType> From<TType>(ISqlField field)
        {
            Contract.Ensures(Contract.Result<SqlFilterField<TEntity, TType>>() != null);
            Guard.IsNotNull(field);
            return new SqlFilterField<TEntity, TType>(field.ShortView);
        }

        public SqlFilterField<TEntity, TType> And<TType>(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilterField<TEntity, TType>>() != null);
            Guard.IsNotNull(field);
            alias = CheckAlias(alias);
            return AddFilter<TType>(GetFieldName(field, alias), AND);
        }

        public SqlFilterField<TEntity, TType> Or<TType>(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilterField<TEntity, TType>>() != null);
            Guard.IsNotNull(field);
            alias = CheckAlias(alias);
            return AddFilter<TType>(GetFieldName(field, alias), OR);
        }

        public SqlFilterField<TEntity, TType> And<TType>(ISqlField field)
        {
            Contract.Ensures(Contract.Result<SqlFilterField<TEntity, TType>>() != null);
            Guard.IsNotNull(field);
            return AddFilter<TType>(field.ShortView, AND);
        }

        public SqlFilterField<TEntity, TType> Or<TType>(ISqlField field)
        {
            Contract.Ensures(Contract.Result<SqlFilterField<TEntity, TType>>() != null);
            Guard.IsNotNull(field);
            return AddFilter<TType>(field.ShortView, OR);
        }

        public SqlFilter<TEntity> And(ISqlFilter filter)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(filter);
            return new SqlFilter<TEntity>(Filter + AND + filter.Filter);
        }

        public SqlFilter<TEntity> Or(ISqlFilter filter)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(filter);
            return new SqlFilter<TEntity>(Filter + OR + filter.Filter);
        }

        public SqlFilter<TEntity> AndGroup(ISqlFilter filter)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(filter);
            return new SqlFilter<TEntity>($"{Filter}{AND}({filter.Filter})");
        }

        public SqlFilter<TEntity> OrGroup(ISqlFilter filter)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(filter);
            return new SqlFilter<TEntity>($"{Filter}{OR}({filter.Filter})");
        }
    }
}

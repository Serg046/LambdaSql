using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using GuardExtensions;

namespace SqlSelectBuilder.SqlFilter
{
    public class SqlFilterField<TEntity, TType> : RestrictedSqlFilterField<TEntity, TType, SqlFilter<TEntity>>
    {
        internal SqlFilterField(string field) : base(field)
        {
            Contract.Requires(field.IsNotEmpty());
        }

        internal SqlFilterField(string filter, string field) : base(filter, field)
        {
            Guard.IsNotEmpty(filter);
            Guard.IsNotEmpty(field);
        }

        protected override SqlFilter<TEntity> BuildFilter(string filter)
        {
            return new SqlFilter<TEntity>(filter);
        }

        //----------------------------------------------------------------------------

        public SqlFilter<TEntity> EqualTo(ISqlField field)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            return LogicFilter("=", field.ShortView);
        }

        public SqlFilter<TEntity> EqualTo<TRight>(Expression<Func<TRight, TType>> field, SqlAlias<TRight> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            alias = CheckAlias(alias);
            return LogicFilter("=", alias.Value + "." + MetadataProvider.Instance.GetPropertyName(field));
        }

        //----------------------------------------------------------------------------

        public SqlFilter<TEntity> NotEqualTo(ISqlField field)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            return LogicFilter("<>", field.ShortView);
        }

        public SqlFilter<TEntity> NotEqualTo<TRight>(Expression<Func<TRight, TType>> field, SqlAlias<TRight> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            alias = CheckAlias(alias);
            return LogicFilter("<>", alias.Value + "." + MetadataProvider.Instance.GetPropertyName(field));
        }

        //----------------------------------------------------------------------------

        public SqlFilter<TEntity> GreaterThan(ISqlField field)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            return LogicFilter(">", field.ShortView);
        }

        public SqlFilter<TEntity> GreaterThan<TRight>(Expression<Func<TRight, TType>> field, SqlAlias<TRight> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            alias = CheckAlias(alias);
            return LogicFilter(">", alias.Value + "." + MetadataProvider.Instance.GetPropertyName(field));
        }

        //----------------------------------------------------------------------------

        public SqlFilter<TEntity> GreaterThanOrEqual(ISqlField field)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            return LogicFilter(">=", field.ShortView);
        }

        public SqlFilter<TEntity> GreaterThanOrEqual<TRight>(Expression<Func<TRight, TType>> field, SqlAlias<TRight> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            alias = CheckAlias(alias);
            return LogicFilter(">=", alias.Value + "." + MetadataProvider.Instance.GetPropertyName(field));
        }

        //----------------------------------------------------------------------------

        public SqlFilter<TEntity> LessThan(ISqlField field)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            return LogicFilter("<", field.ShortView);
        }

        public SqlFilter<TEntity> LessThan<TRight>(Expression<Func<TRight, TType>> field, SqlAlias<TRight> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            alias = CheckAlias(alias);
            return LogicFilter("<", alias.Value + "." + MetadataProvider.Instance.GetPropertyName(field));
        }

        //----------------------------------------------------------------------------

        public SqlFilter<TEntity> LessThanOrEqual(ISqlField field)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            return LogicFilter("<=", field.ShortView);
        }

        public SqlFilter<TEntity> LessThanOrEqual<TRight>(Expression<Func<TRight, TType>> field, SqlAlias<TRight> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            alias = CheckAlias(alias);
            return LogicFilter("<=", alias.Value + "." + MetadataProvider.Instance.GetPropertyName(field));
        }

        //----------------------------------------------------------------------------
    }
}

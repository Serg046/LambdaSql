using System;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using GuardExtensions;

namespace SqlSelectBuilder.SqlFilter
{
    public class SqlFilterField<TEntity, TType> : RestrictedSqlFilterField<TEntity, TType, SqlFilter<TEntity>>
    {
        internal SqlFilterField(ImmutableList<ISqlFilterItem> sqlFilterItems, SqlFilterItem currentItem)
            : base(sqlFilterItems, currentItem)
        {
            Guard.IsNotNull(sqlFilterItems);
            Guard.IsNotNull(currentItem);
        }

        protected override SqlFilter<TEntity> BuildFilter(ImmutableList<ISqlFilterItem> sqlFilterItems)
        {
            return new SqlFilter<TEntity>(sqlFilterItems);
        }

        //----------------------------------------------------------------------------

        public SqlFilter<TEntity> EqualTo(ISqlField field)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            return LogicFilter("=", field);
        }

        public SqlFilter<TEntity> EqualTo<TRight>(Expression<Func<TRight, TType>> field, SqlAlias<TRight> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            alias = CheckAlias(alias);
            return LogicFilter("=", field, alias);
        }

        //----------------------------------------------------------------------------

        public SqlFilter<TEntity> NotEqualTo(ISqlField field)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            return LogicFilter("<>", field);
        }

        public SqlFilter<TEntity> NotEqualTo<TRight>(Expression<Func<TRight, TType>> field, SqlAlias<TRight> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            alias = CheckAlias(alias);
            return LogicFilter("<>", field, alias);
        }

        //----------------------------------------------------------------------------

        public SqlFilter<TEntity> GreaterThan(ISqlField field)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            return LogicFilter(">", field);
        }

        public SqlFilter<TEntity> GreaterThan<TRight>(Expression<Func<TRight, TType>> field, SqlAlias<TRight> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            alias = CheckAlias(alias);
            return LogicFilter(">", field, alias);
        }

        //----------------------------------------------------------------------------

        public SqlFilter<TEntity> GreaterThanOrEqual(ISqlField field)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            return LogicFilter(">=", field);
        }

        public SqlFilter<TEntity> GreaterThanOrEqual<TRight>(Expression<Func<TRight, TType>> field, SqlAlias<TRight> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            alias = CheckAlias(alias);
            return LogicFilter(">=", field, alias);
        }

        //----------------------------------------------------------------------------

        public SqlFilter<TEntity> LessThan(ISqlField field)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            return LogicFilter("<", field);
        }

        public SqlFilter<TEntity> LessThan<TRight>(Expression<Func<TRight, TType>> field, SqlAlias<TRight> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            alias = CheckAlias(alias);
            return LogicFilter("<", field, alias);
        }

        //----------------------------------------------------------------------------

        public SqlFilter<TEntity> LessThanOrEqual(ISqlField field)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            return LogicFilter("<=", field);
        }

        public SqlFilter<TEntity> LessThanOrEqual<TRight>(Expression<Func<TRight, TType>> field, SqlAlias<TRight> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            alias = CheckAlias(alias);
            return LogicFilter("<=", field, alias);
        }

        //----------------------------------------------------------------------------
    }
}

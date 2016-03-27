using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GuardExtensions;

namespace SqlSelectBuilder.SqlFilter
{
    public class RestrictedSqlFilterField<TEntity, TType, TResult>
        where TResult : SqlFilterBase<TEntity>
    {
        private readonly string _filter;
        private readonly string _field;

        internal RestrictedSqlFilterField(string field)
        {
            Contract.Requires(field.IsNotEmpty());
            _filter = string.Empty;
            _field = field;
        }

        internal RestrictedSqlFilterField(string filter, string field)
        {
            Guard.IsNotEmpty(filter);
            Guard.IsNotEmpty(field);
            _filter = filter;
            _field = field;
        }

        protected virtual TResult BuildFilter(string filter)
        {
            return new RestrictedSqlFilter<TEntity>(filter) as TResult;
        }

        protected SqlAlias<T> CheckAlias<T>(SqlAlias<T> alias)
        {
            return alias ?? MetadataProvider.Instance.AliasFor<T>();
        }

        public TResult SatisfyLambda(Func<string, IParameterConverter, string> filter)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(filter);
            return BuildFilter(_filter + filter(_field, MetadataProvider.Instance));
        }

        public TResult IsNull()
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            return BuildFilter(_filter + _field + " IS NULL");
        }

        public TResult IsNotNull()
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            return BuildFilter(_filter + _field + " IS NOT NULL");
        }

        public TResult Like(string value)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotEmpty(value);
            return BuildFilter($"{_filter}{_field} LIKE {MetadataProvider.Instance.ParameterToString(value)}");
        }

        //----------------------------------------------------------------------------

        protected TResult LogicFilter(string logicOperator, string value)
        {
            Contract.Requires(logicOperator.IsNotEmpty());
            Contract.Requires(value.IsNotEmpty());
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            return BuildFilter($"{_filter}{_field} {logicOperator} {value}");
        }

        //----------------------------------------------------------------------------

        public TResult EqualTo(TType value)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            return LogicFilter("=", MetadataProvider.Instance.ParameterToString(value));
        }

        public TResult EqualTo(SqlField<TEntity> field)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            return LogicFilter("=", field.ShortView);
        }

        public TResult EqualTo(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            alias = CheckAlias(alias);
            return LogicFilter("=", alias.Value + "." + MetadataProvider.Instance.GetPropertyName(field));
        }

        //----------------------------------------------------------------------------

        public TResult NotEqualTo(TType value)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            return LogicFilter("<>", MetadataProvider.Instance.ParameterToString(value));
        }

        public TResult NotEqualTo(SqlField<TEntity> field)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            return LogicFilter("<>", field.ShortView);
        }

        public TResult NotEqualTo(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            alias = CheckAlias(alias);
            return LogicFilter("<>", alias.Value + "." + MetadataProvider.Instance.GetPropertyName(field));
        }

        //----------------------------------------------------------------------------

        public TResult GreaterThan(TType value)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            return LogicFilter(">", MetadataProvider.Instance.ParameterToString(value));
        }

        public TResult GreaterThan(SqlField<TEntity> field)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            return LogicFilter(">", field.ShortView);
        }

        public TResult GreaterThan(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            alias = CheckAlias(alias);
            return LogicFilter(">", alias.Value + "." + MetadataProvider.Instance.GetPropertyName(field));
        }

        //----------------------------------------------------------------------------

        public TResult GreaterThanOrEqual(TType value)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            return LogicFilter(">=", MetadataProvider.Instance.ParameterToString(value));
        }

        public TResult GreaterThanOrEqual(SqlField<TEntity> field)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            return LogicFilter(">=", field.ShortView);
        }

        public TResult GreaterThanOrEqual(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            alias = CheckAlias(alias);
            return LogicFilter(">=", alias.Value + "." + MetadataProvider.Instance.GetPropertyName(field));
        }

        //----------------------------------------------------------------------------

        public TResult LessThan(TType value)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            return LogicFilter("<", MetadataProvider.Instance.ParameterToString(value));
        }

        public TResult LessThan(SqlField<TEntity> field)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            return LogicFilter("<", field.ShortView);
        }

        public TResult LessThan(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            alias = CheckAlias(alias);
            return LogicFilter("<", alias.Value + "." + MetadataProvider.Instance.GetPropertyName(field));
        }

        //----------------------------------------------------------------------------

        public TResult LessThanOrEqual(TType value)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            return LogicFilter("<=", MetadataProvider.Instance.ParameterToString(value));
        }

        public TResult LessThanOrEqual(SqlField<TEntity> field)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            return LogicFilter("<=", field.ShortView);
        }

        public TResult LessThanOrEqual(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            alias = CheckAlias(alias);
            return LogicFilter("<=", alias.Value + "." + MetadataProvider.Instance.GetPropertyName(field));
        }

        //----------------------------------------------------------------------------

        public TResult In(params TType[] values)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(values);
            Guard.IsPositive(values.Length);
            return In((IEnumerable<TType>)values);
        }

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public TResult In(IEnumerable<TType> values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (!values.Any())
                throw new ArgumentException("Collection is empty");
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Contract.EndContractBlock();
            var parameters = string.Join(",", values.Select(v => MetadataProvider.Instance.ParameterToString(v)));
            return BuildFilter($"{_filter}{_field} IN ({parameters})");
        }

        //----------------------------------------------------------------------------
    }
}

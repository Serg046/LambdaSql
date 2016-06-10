using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using GuardExtensions;
// ReSharper disable CheckNamespace

namespace SqlSelectBuilder
{
    public class RestrictedSqlFilterField<TEntity, TType, TResult>
        where TResult : SqlFilterBase<TEntity>
    {
        private readonly ImmutableList<ISqlFilterItem> _sqlFilterItems;
        private readonly SqlFilterItem _currentItem;

        internal RestrictedSqlFilterField(ImmutableList<ISqlFilterItem> sqlFilterItems, SqlFilterItem currentItem)
        {
            Guard.IsNotNull(sqlFilterItems);
            Guard.IsNotNull(currentItem);
            _sqlFilterItems = sqlFilterItems;
            _currentItem = currentItem;
        }

        protected virtual TResult BuildFilter(ImmutableList<ISqlFilterItem> sqlFilterItems)
        {
            return new RestrictedSqlFilter<TEntity>(sqlFilterItems) as TResult;
        }

        protected TResult BuildFilter(string expression, params object[] args)
        {
            return BuildFilter(_sqlFilterItems.Add(_currentItem.SetExpression(expression, args)));
        }

        protected SqlAlias<T> CheckAlias<T>(SqlAlias<T> alias)
        {
            return alias ?? MetadataProvider.Instance.AliasFor<T>();
        }

        public TResult SatisfyLambda(Func<ISqlField, IParameterConverter, string> filter)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(filter);
            return BuildFilter(filter(_currentItem.SqlField, MetadataProvider.Instance));
        }

        public TResult IsNull()
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            return BuildFilter("{0} IS NULL", _currentItem.SqlField);
        }

        public TResult IsNotNull()
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            return BuildFilter("{0} IS NOT NULL", _currentItem.SqlField);
        }

        public TResult Like(string value)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotEmpty(value);
            return BuildFilter("{0} LIKE " + MetadataProvider.Instance.ParameterToString(value),
                _currentItem.SqlField);
        }

        //----------------------------------------------------------------------------

        protected TResult LogicFilter(string logicOperator, string value)
        {
            Contract.Requires(logicOperator.IsNotEmpty());
            Contract.Requires(value.IsNotEmpty());
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            return BuildFilter("{0} " + logicOperator + " " + value, _currentItem.SqlField);
        }

        protected TResult LogicFilter(string logicOperator, ISqlField sqlField)
        {
            Contract.Requires(logicOperator.IsNotEmpty());
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(sqlField);
            return BuildFilter("{0} " + logicOperator + " {1}", _currentItem.SqlField, sqlField);
        }

        protected TResult LogicFilter<T>(string logicOperator, LambdaExpression field, SqlAlias<T> alias)
        {
            Contract.Requires(logicOperator.IsNotEmpty());
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);

            alias = CheckAlias(alias);
            var sqlField = new SqlField<T>() {Alias = alias, Name = MetadataProvider.Instance.GetPropertyName(field)};
            return LogicFilter(logicOperator, sqlField);
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
            return LogicFilter("=", field);
        }

        public TResult EqualTo(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            return LogicFilter("=", field, alias);
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
            return LogicFilter("<>", field);
        }

        public TResult NotEqualTo(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            return LogicFilter("<>", field, alias);
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
            return LogicFilter(">", field);
        }

        public TResult GreaterThan(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            return LogicFilter(">", field, alias);
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
            return LogicFilter(">=", field);
        }

        public TResult GreaterThanOrEqual(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            return LogicFilter(">=", field, alias);
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
            return LogicFilter("<", field);
        }

        public TResult LessThan(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            return LogicFilter("<", field, alias);
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
            return LogicFilter("<=", field);
        }

        public TResult LessThanOrEqual(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            return LogicFilter("<=", field, alias);
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
            return BuildFilter("{0} IN (" + parameters + ")", _currentItem.SqlField);
        }

        //----------------------------------------------------------------------------

        public TResult NotIn(params TType[] values)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(values);
            Guard.IsPositive(values.Length);
            return NotIn((IEnumerable<TType>)values);
        }

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public TResult NotIn(IEnumerable<TType> values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (!values.Any())
                throw new ArgumentException("Collection is empty");
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Contract.EndContractBlock();
            var parameters = string.Join(",", values.Select(v => MetadataProvider.Instance.ParameterToString(v)));
            return BuildFilter("{0} NOT IN (" + parameters + ")", _currentItem.SqlField);
        }

        //----------------------------------------------------------------------------
    }
}

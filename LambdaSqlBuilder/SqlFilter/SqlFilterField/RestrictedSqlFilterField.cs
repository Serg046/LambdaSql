using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using GuardExtensions;

namespace LambdaSqlBuilder.SqlFilter.SqlFilterField
{
    public class RestrictedSqlFilterField<TEntity, TType, TResult>
        where TResult : SqlFilterBase
    {
        private readonly ImmutableList<SqlFilterItemFunc> _sqlFilterItems;
        private readonly ISqlField _sqlField;

        internal RestrictedSqlFilterField(ImmutableList<SqlFilterItemFunc> sqlFilterItems, ISqlField sqlField)
        {
            _sqlFilterItems = sqlFilterItems;
            _sqlField = sqlField;
        }

        internal virtual TResult BuildFilter(ImmutableList<SqlFilterItemFunc> sqlFilterItems)
            => new RestrictedSqlFilter<TEntity>(sqlFilterItems) as TResult;

        protected TResult BuildFilter(string expression, params object[] args)
            => BuildFilter(_sqlFilterItems.Add(config => new SqlFilterItem.SqlFilterItem(config, expression, args)));

        protected SqlAlias<T> CheckAlias<T>(SqlAlias<T> alias)
            => alias ?? MetadataProvider.Instance.AliasFor<T>();

        //----------------------------------------------------------------------------

        public TResult SatisfyLambda(Func<ISqlField, IParameterConverter, string> filter)
        {
            Guard.IsNotNull(filter);
            return BuildFilter(filter(_sqlField, MetadataProvider.Instance));
        }

        //----------------------------------------------------------------------------

        public TResult IsNull() => BuildFilter("{0} IS NULL", _sqlField);

        public TResult IsNotNull() => BuildFilter("{0} IS NOT NULL", _sqlField);

        public TResult Like(string value)
        {
            Guard.IsNotEmpty(value);
            return BuildFilter("{0} LIKE " + MetadataProvider.Instance.ParameterToString(value), _sqlField);
        }

        //----------------------------------------------------------------------------

        protected TResult LogicFilter(string logicOperator, string value)
            => BuildFilter("{0} " + logicOperator + " " + value, _sqlField);

        protected TResult LogicFilter(string logicOperator, ISqlField sqlField)
        {
            Guard.IsNotNull(sqlField);
            return BuildFilter("{0} " + logicOperator + " {1}", _sqlField, sqlField);
        }

        protected TResult LogicFilter<T>(string logicOperator, LambdaExpression field, SqlAlias<T> alias)
        {
            Guard.IsNotNull(field);

            alias = CheckAlias(alias);
            var sqlField = new SqlField<T>() {Alias = alias, Name = MetadataProvider.Instance.GetPropertyName(field)};
            return LogicFilter(logicOperator, sqlField);
        }

        //----------------------------------------------------------------------------

        public TResult EqualTo(TType value) => LogicFilter("=", MetadataProvider.Instance.ParameterToString(value));

        public TResult EqualTo(SqlField<TEntity> field) => LogicFilter("=", field);

        public TResult EqualTo(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null) => LogicFilter("=", field, alias);

        //----------------------------------------------------------------------------

        public TResult NotEqualTo(TType value) => LogicFilter("<>", MetadataProvider.Instance.ParameterToString(value));

        public TResult NotEqualTo(SqlField<TEntity> field) => LogicFilter("<>", field);

        public TResult NotEqualTo(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null) => LogicFilter("<>", field, alias);

        //----------------------------------------------------------------------------

        public TResult GreaterThan(TType value) => LogicFilter(">", MetadataProvider.Instance.ParameterToString(value));

        public TResult GreaterThan(SqlField<TEntity> field) => LogicFilter(">", field);

        public TResult GreaterThan(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null) => LogicFilter(">", field, alias);

        //----------------------------------------------------------------------------

        public TResult GreaterThanOrEqual(TType value) => LogicFilter(">=", MetadataProvider.Instance.ParameterToString(value));

        public TResult GreaterThanOrEqual(SqlField<TEntity> field) => LogicFilter(">=", field);

        public TResult GreaterThanOrEqual(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null) => LogicFilter(">=", field, alias);

        //----------------------------------------------------------------------------

        public TResult LessThan(TType value) => LogicFilter("<", MetadataProvider.Instance.ParameterToString(value));

        public TResult LessThan(SqlField<TEntity> field) => LogicFilter("<", field);

        public TResult LessThan(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null) => LogicFilter("<", field, alias);

        //----------------------------------------------------------------------------

        public TResult LessThanOrEqual(TType value) => LogicFilter("<=", MetadataProvider.Instance.ParameterToString(value));

        public TResult LessThanOrEqual(SqlField<TEntity> field) => LogicFilter("<=", field);

        public TResult LessThanOrEqual(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null) => LogicFilter("<=", field, alias);

        //----------------------------------------------------------------------------

        public TResult In(params TType[] values)
        {
            Guard.IsNotNull(values);
            Guard.IsPositive(values.Length);
            return In((IEnumerable<TType>)values);
        }

        public TResult In(IEnumerable<TType> values)
        {
            Guard.IsNotNull(values);
            if (!values.Any())
                throw new ArgumentException("Collection is empty");

            var parameters = string.Join(",", values.Select(v => MetadataProvider.Instance.ParameterToString(v)));
            return BuildFilter("{0} IN (" + parameters + ")", _sqlField);
        }

        //----------------------------------------------------------------------------

        public TResult NotIn(params TType[] values)
        {
            Guard.IsNotNull(values);
            Guard.IsPositive(values.Length);
            return NotIn((IEnumerable<TType>)values);
        }

        public TResult NotIn(IEnumerable<TType> values)
        {
            Guard.IsNotNull(values);
            if (!values.Any())
                throw new ArgumentException("Collection is empty");

            var parameters = string.Join(",", values.Select(v => MetadataProvider.Instance.ParameterToString(v)));
            return BuildFilter("{0} NOT IN (" + parameters + ")", _sqlField);
        }
    }
}

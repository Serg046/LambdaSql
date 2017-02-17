using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using GuardExtensions;
using LambdaSqlBuilder.SqlFilter.SqlFilterItem;

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

        internal TResult BuildFilter(string expression, ISqlField sqlField)
            => BuildFilter(_sqlFilterItems.Add(config
                => new SqlFilterItem.SqlFilterItem(expression, SqlFilterParameter.Create(config, sqlField))));

        internal TResult BuildFilter(string expression, params SqlFilterParameter[] args)
            => BuildFilter(_sqlFilterItems.Add(config => new SqlFilterItem.SqlFilterItem(expression, args)));

        internal TResult BuildFilter(string expression, Func<SqlFilterConfiguration, SqlFilterParameter[]> args)
            => BuildFilter(_sqlFilterItems.Add(config => new SqlFilterItem.SqlFilterItem(expression, args.Invoke(config))));

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

        protected TResult ComparisonFilter(string logicOperator, object value)
        {
            Func<SqlFilterConfiguration, SqlFilterParameter[]> args = config => new[]
            {
                SqlFilterParameter.Create(config, _sqlField),
                SqlFilterParameter.Create(config, value)
            };
            return BuildFilter("{0} " + logicOperator + " {1}", args);
        }

        protected TResult ComparisonFilter(string logicOperator, ISqlField sqlField)
        {
            Guard.IsNotNull(sqlField);
            Func<SqlFilterConfiguration, SqlFilterParameter[]> args = config => new[]
            {
                SqlFilterParameter.Create(config, _sqlField),
                SqlFilterParameter.Create(config, sqlField)
            };
            return BuildFilter("{0} " + logicOperator + " {1}", args);
        }

        protected TResult ComparisonFilter<T>(string logicOperator, LambdaExpression field, SqlAlias<T> alias)
        {
            Guard.IsNotNull(field);

            alias = CheckAlias(alias);
            var sqlField = new SqlField<T>() {Alias = alias, Name = MetadataProvider.Instance.GetPropertyName(field)};
            return ComparisonFilter(logicOperator, sqlField);
        }

        //----------------------------------------------------------------------------

        public TResult EqualTo(TType value) => ComparisonFilter("=", value);

        public TResult EqualTo(SqlField<TEntity> field) => ComparisonFilter("=", field);

        public TResult EqualTo(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null) => ComparisonFilter("=", field, alias);

        //----------------------------------------------------------------------------

        public TResult NotEqualTo(TType value) => ComparisonFilter("<>", value);

        public TResult NotEqualTo(SqlField<TEntity> field) => ComparisonFilter("<>", field);

        public TResult NotEqualTo(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null) => ComparisonFilter("<>", field, alias);

        //----------------------------------------------------------------------------

        public TResult GreaterThan(TType value) => ComparisonFilter(">", value);

        public TResult GreaterThan(SqlField<TEntity> field) => ComparisonFilter(">", field);

        public TResult GreaterThan(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null) => ComparisonFilter(">", field, alias);

        //----------------------------------------------------------------------------

        public TResult GreaterThanOrEqual(TType value) => ComparisonFilter(">=", value);

        public TResult GreaterThanOrEqual(SqlField<TEntity> field) => ComparisonFilter(">=", field);

        public TResult GreaterThanOrEqual(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null) => ComparisonFilter(">=", field, alias);

        //----------------------------------------------------------------------------

        public TResult LessThan(TType value) => ComparisonFilter("<", value);

        public TResult LessThan(SqlField<TEntity> field) => ComparisonFilter("<", field);

        public TResult LessThan(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null) => ComparisonFilter("<", field, alias);

        //----------------------------------------------------------------------------

        public TResult LessThanOrEqual(TType value) => ComparisonFilter("<=", value);

        public TResult LessThanOrEqual(SqlField<TEntity> field) => ComparisonFilter("<=", field);

        public TResult LessThanOrEqual(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null) => ComparisonFilter("<=", field, alias);

        //----------------------------------------------------------------------------

        public TResult ContainsFilter(string @operator, IEnumerable<TType> values)
        {
            Func<SqlFilterConfiguration, SqlFilterParameter[]> args = config =>
            {
                var list = new List<SqlFilterParameter>();
                list.Add(SqlFilterParameter.Create(config, _sqlField));
                list.AddRange(values.Select(val => SqlFilterParameter.Create(config, val)));
                return list.ToArray();
            };

            var parameters = string.Join(",", values.Select((val, i) => $"{{{i + 1}}}"));
            return BuildFilter("{0} "+ @operator + " (" + parameters + ")", args);
        }

        //----


        public TResult In(params TType[] values)
        {
            Guard.IsNotNull(values);
            Guard.IsPositive(values.Length);
            return ContainsFilter("IN", values);
        }

        public TResult In(IEnumerable<TType> values)
        {
            Guard.IsNotNull(values);
            if (!values.Any())
                throw new ArgumentException("Collection is empty");

            return ContainsFilter("IN", values);
        }

        //----------------------------------------------------------------------------

        public TResult NotIn(params TType[] values)
        {
            Guard.IsNotNull(values);
            Guard.IsPositive(values.Length);
            return ContainsFilter("NOT IN", values);
        }

        public TResult NotIn(IEnumerable<TType> values)
        {
            Guard.IsNotNull(values);
            if (!values.Any())
                throw new ArgumentException("Collection is empty");

            return ContainsFilter("NOT IN", values);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using GuardExtensions;
using LambdaSqlBuilder.SqlFilter.SqlFilterItem;

namespace LambdaSqlBuilder.SqlFilter
{
    public class SqlFilterField<TEntity, TType, TResult>
        where TResult : SqlFilterBase
    {
        private readonly ISqlField _sqlField;
        private readonly SqlFilterBuilder<TEntity> _sqlFilterBuilder;

        internal SqlFilterField(ImmutableList<SqlFilterItemFunc> sqlFilterItems, ISqlField sqlField,
            Func<ImmutableList<SqlFilterItemFunc>, SqlFilterBase> filterCreatorFunc)
        {
            _sqlField = sqlField;
            _sqlFilterBuilder = new SqlFilterBuilder<TEntity>(sqlFilterItems, sqlField, filterCreatorFunc);
        }

        //----------------------------------------------------------------------------

        public TResult SatisfyLambda(Func<ISqlField, string> filter)
        {
            Guard.IsNotNull(filter);
            return _sqlFilterBuilder.BuildFilter<TResult>(filter(_sqlField));
        }

        //----------------------------------------------------------------------------

        public TResult IsNull() => _sqlFilterBuilder.BuildFilter<TResult>("{0} IS NULL", _sqlField);

        public TResult IsNotNull() => _sqlFilterBuilder.BuildFilter<TResult>("{0} IS NOT NULL", _sqlField);

        //----------------------------------------------------------------------------

        public TResult Like(string value)
        {
            Guard.IsNotEmpty(value);
            Func<SqlFilterConfiguration, SqlFilterParameter[]> args = config => new[]
            {
                SqlFilterParameter.Create(config, _sqlField),
                SqlFilterParameter.Create(config, value)
            };
            return _sqlFilterBuilder.BuildFilter<TResult>("{0} LIKE {1}", args);
        }

        //----------------------------------------------------------------------------

        public TResult EqualTo(TType value) => _sqlFilterBuilder.ComparisonFilter<TResult>("=", value);

        public TResult EqualTo(SqlField<TEntity> field) => _sqlFilterBuilder.ComparisonFilter<TResult>("=", field);

        public TResult EqualTo(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
            => _sqlFilterBuilder.ComparisonFilter<TResult>("=", field, alias);

        public SqlFilter<TEntity> EqualTo(ISqlField field) => _sqlFilterBuilder.ComparisonFilter<SqlFilter<TEntity>>("=", field);

        public SqlFilter<TEntity> EqualTo<TRight>(Expression<Func<TRight, TType>> field, SqlAlias<TRight> alias = null)
            => _sqlFilterBuilder.ComparisonFilter<SqlFilter<TEntity>, TRight>("=", field, alias);

        //----------------------------------------------------------------------------

        public TResult NotEqualTo(TType value) => _sqlFilterBuilder.ComparisonFilter<TResult>("<>", value);

        public TResult NotEqualTo(SqlField<TEntity> field) => _sqlFilterBuilder.ComparisonFilter<TResult>("<>", field);

        public TResult NotEqualTo(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
            => _sqlFilterBuilder.ComparisonFilter<TResult>("<>", field, alias);

        public SqlFilter<TEntity> NotEqualTo(ISqlField field) => _sqlFilterBuilder.ComparisonFilter<SqlFilter<TEntity>>("<>", field);

        public SqlFilter<TEntity> NotEqualTo<TRight>(Expression<Func<TRight, TType>> field, SqlAlias<TRight> alias = null)
            => _sqlFilterBuilder.ComparisonFilter<SqlFilter<TEntity>, TRight>("<>", field, alias);

        //----------------------------------------------------------------------------

        public TResult GreaterThan(TType value) => _sqlFilterBuilder.ComparisonFilter<TResult>(">", value);

        public TResult GreaterThan(SqlField<TEntity> field) => _sqlFilterBuilder.ComparisonFilter<TResult>(">", field);

        public TResult GreaterThan(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(">", field, alias);

        public SqlFilter<TEntity> GreaterThan(ISqlField field) => _sqlFilterBuilder.ComparisonFilter<SqlFilter<TEntity>>(">", field);

        public SqlFilter<TEntity> GreaterThan<TRight>(Expression<Func<TRight, TType>> field, SqlAlias<TRight> alias = null)
            => _sqlFilterBuilder.ComparisonFilter<SqlFilter<TEntity>, TRight>(">", field, alias);

        //----------------------------------------------------------------------------

        public TResult GreaterThanOrEqual(TType value) => _sqlFilterBuilder.ComparisonFilter<TResult>(">=", value);

        public TResult GreaterThanOrEqual(SqlField<TEntity> field) => _sqlFilterBuilder.ComparisonFilter<TResult>(">=", field);

        public TResult GreaterThanOrEqual(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(">=", field, alias);

        public SqlFilter<TEntity> GreaterThanOrEqual(ISqlField field) => _sqlFilterBuilder.ComparisonFilter<SqlFilter<TEntity>>(">=", field);

        public SqlFilter<TEntity> GreaterThanOrEqual<TRight>(Expression<Func<TRight, TType>> field, SqlAlias<TRight> alias = null)
            => _sqlFilterBuilder.ComparisonFilter<SqlFilter<TEntity>, TRight>(">=", field, alias);

        //----------------------------------------------------------------------------

        public TResult LessThan(TType value) => _sqlFilterBuilder.ComparisonFilter<TResult>("<", value);

        public TResult LessThan(SqlField<TEntity> field) => _sqlFilterBuilder.ComparisonFilter<TResult>("<", field);

        public TResult LessThan(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
            => _sqlFilterBuilder.ComparisonFilter<TResult>("<", field, alias);

        public SqlFilter<TEntity> LessThan(ISqlField field) => _sqlFilterBuilder.ComparisonFilter<SqlFilter<TEntity>>("<", field);

        public SqlFilter<TEntity> LessThan<TRight>(Expression<Func<TRight, TType>> field, SqlAlias<TRight> alias = null)
            => _sqlFilterBuilder.ComparisonFilter<SqlFilter<TEntity>, TRight>("<", field, alias);

        //----------------------------------------------------------------------------

        public TResult LessThanOrEqual(TType value) => _sqlFilterBuilder.ComparisonFilter<TResult>("<=", value);

        public TResult LessThanOrEqual(SqlField<TEntity> field) => _sqlFilterBuilder.ComparisonFilter<TResult>("<=", field);

        public TResult LessThanOrEqual(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
            => _sqlFilterBuilder.ComparisonFilter<TResult>("<=", field, alias);

        public SqlFilter<TEntity> LessThanOrEqual(ISqlField field) => _sqlFilterBuilder.ComparisonFilter<SqlFilter<TEntity>>("<=", field);

        public SqlFilter<TEntity> LessThanOrEqual<TRight>(Expression<Func<TRight, TType>> field, SqlAlias<TRight> alias = null)
            => _sqlFilterBuilder.ComparisonFilter<SqlFilter<TEntity>, TRight>("<=", field, alias);

        //----------------------------------------------------------------------------

        public TResult In(params TType[] values)
        {
            Guard.IsNotNull(values);
            Guard.IsPositive(values.Length);
            return _sqlFilterBuilder.ContainsFilter<TResult, TType>("IN", values);
        }

        public TResult In(IEnumerable<TType> values)
        {
            Guard.IsNotNull(values);
            if (!values.Any())
                throw new ArgumentException("Collection is empty");

            return _sqlFilterBuilder.ContainsFilter<TResult, TType>("IN", values);
        }

        //----------------------------------------------------------------------------

        public TResult NotIn(params TType[] values)
        {
            Guard.IsNotNull(values);
            Guard.IsPositive(values.Length);
            return _sqlFilterBuilder.ContainsFilter<TResult, TType>("NOT IN", values);
        }

        public TResult NotIn(IEnumerable<TType> values)
        {
            Guard.IsNotNull(values);
            if (!values.Any())
                throw new ArgumentException("Collection is empty");

            return _sqlFilterBuilder.ContainsFilter<TResult, TType>("NOT IN", values);
        }
    }
}

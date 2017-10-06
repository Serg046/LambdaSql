using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using GuardExtensions;
using LambdaSqlBuilder.Field;
using LambdaSqlBuilder.Filter.SqlFilterItem;

namespace LambdaSqlBuilder.Filter
{
    public class SqlFilterField<TEntity, TFieldType, TResult>
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

            SqlFilterParameter[] GetParameters(SqlFilterConfiguration config) => new[]
            {
                SqlFilterParameter.Create(config, _sqlField), SqlFilterParameter.Create(config, value)
            };

            return _sqlFilterBuilder.BuildFilter<TResult>("{0} LIKE {1}", GetParameters);
        }

        //----------------------------------------------------------------------------
        
        private const string EQUAL_OPERATOR = "=";

        public TResult EqualTo(TFieldType value)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(EQUAL_OPERATOR, value);

        public TResult EqualTo(Expression<Func<TEntity, TFieldType>> field, SqlAlias<TEntity> alias = null)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(EQUAL_OPERATOR, field, alias);

        public TResult EqualTo(SqlField<TEntity, TFieldType> field)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(EQUAL_OPERATOR, field);

        public SqlFilter<TEntity> EqualTo<TRight>(Expression<Func<TRight, TFieldType>> field, SqlAlias<TRight> alias = null)
            => _sqlFilterBuilder.ComparisonFilter<SqlFilter<TEntity>, TRight>(EQUAL_OPERATOR, field, alias);

        public SqlFilter<TEntity> EqualTo<TRight>(SqlField<TRight, TFieldType> field)
            => _sqlFilterBuilder.ComparisonFilter<SqlFilter<TEntity>>(EQUAL_OPERATOR, field);

        public SqlFilter<TEntity> EqualTo(ISqlField field)
            => _sqlFilterBuilder.ComparisonFilter<SqlFilter<TEntity>>(EQUAL_OPERATOR, field);


        //----------------------------------------------------------------------------

        private const string NOT_EQUAL_OPERATOR = "<>";

        public TResult NotEqualTo(TFieldType value)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(NOT_EQUAL_OPERATOR, value);

        public TResult NotEqualTo(Expression<Func<TEntity, TFieldType>> field, SqlAlias<TEntity> alias = null)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(NOT_EQUAL_OPERATOR, field, alias);

        public TResult NotEqualTo(SqlField<TEntity, TFieldType> field)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(NOT_EQUAL_OPERATOR, field);

        public SqlFilter<TEntity> NotEqualTo<TRight>(Expression<Func<TRight, TFieldType>> field, SqlAlias<TRight> alias = null)
            => _sqlFilterBuilder.ComparisonFilter<SqlFilter<TEntity>, TRight>(NOT_EQUAL_OPERATOR, field, alias);

        public SqlFilter<TEntity> NotEqualTo<TRight>(SqlField<TRight, TFieldType> field)
            => _sqlFilterBuilder.ComparisonFilter<SqlFilter<TEntity>>(NOT_EQUAL_OPERATOR, field);

        public SqlFilter<TEntity> NotEqualTo(ISqlField field)
            => _sqlFilterBuilder.ComparisonFilter<SqlFilter<TEntity>>(NOT_EQUAL_OPERATOR, field);

        //----------------------------------------------------------------------------
        
        private const string GREATER_THAN_OPERATOR = ">";

        public TResult GreaterThan(TFieldType value)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(GREATER_THAN_OPERATOR, value);

        public TResult GreaterThan(Expression<Func<TEntity, TFieldType>> field, SqlAlias<TEntity> alias = null)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(GREATER_THAN_OPERATOR, field, alias);

        public TResult GreaterThan(SqlField<TEntity, TFieldType> field)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(GREATER_THAN_OPERATOR, field);

        public SqlFilter<TEntity> GreaterThan<TRight>(Expression<Func<TRight, TFieldType>> field, SqlAlias<TRight> alias = null)
            => _sqlFilterBuilder.ComparisonFilter<SqlFilter<TEntity>, TRight>(GREATER_THAN_OPERATOR, field, alias);

        public SqlFilter<TEntity> GreaterThan<TRight>(SqlField<TRight, TFieldType> field)
            => _sqlFilterBuilder.ComparisonFilter<SqlFilter<TEntity>>(GREATER_THAN_OPERATOR, field);

        public SqlFilter<TEntity> GreaterThan(ISqlField field)
            => _sqlFilterBuilder.ComparisonFilter<SqlFilter<TEntity>>(GREATER_THAN_OPERATOR, field);

        //----------------------------------------------------------------------------

        private const string GREATER_THAN_OR_EQUAL_OPERATOR = ">=";

        public TResult GreaterThanOrEqual(TFieldType value)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(GREATER_THAN_OR_EQUAL_OPERATOR, value);

        public TResult GreaterThanOrEqual(Expression<Func<TEntity, TFieldType>> field, SqlAlias<TEntity> alias = null)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(GREATER_THAN_OR_EQUAL_OPERATOR, field, alias);

        public TResult GreaterThanOrEqual(SqlField<TEntity, TFieldType> field)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(GREATER_THAN_OR_EQUAL_OPERATOR, field);

        public SqlFilter<TEntity> GreaterThanOrEqual<TRight>(Expression<Func<TRight, TFieldType>> field, SqlAlias<TRight> alias = null)
            => _sqlFilterBuilder.ComparisonFilter<SqlFilter<TEntity>, TRight>(GREATER_THAN_OR_EQUAL_OPERATOR, field, alias);

        public SqlFilter<TEntity> GreaterThanOrEqual<TRight>(SqlField<TRight, TFieldType> field)
            => _sqlFilterBuilder.ComparisonFilter<SqlFilter<TEntity>>(GREATER_THAN_OR_EQUAL_OPERATOR, field);

        public SqlFilter<TEntity> GreaterThanOrEqual(ISqlField field)
            => _sqlFilterBuilder.ComparisonFilter<SqlFilter<TEntity>>(GREATER_THAN_OR_EQUAL_OPERATOR, field);

        //----------------------------------------------------------------------------

        private const string LESS_THAN_OPERATOR = "<";

        public TResult LessThan(TFieldType value)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(LESS_THAN_OPERATOR, value);

        public TResult LessThan(Expression<Func<TEntity, TFieldType>> field, SqlAlias<TEntity> alias = null)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(LESS_THAN_OPERATOR, field, alias);

        public TResult LessThan(SqlField<TEntity, TFieldType> field)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(LESS_THAN_OPERATOR, field);

        public SqlFilter<TEntity> LessThan<TRight>(Expression<Func<TRight, TFieldType>> field, SqlAlias<TRight> alias = null)
            => _sqlFilterBuilder.ComparisonFilter<SqlFilter<TEntity>, TRight>(LESS_THAN_OPERATOR, field, alias);

        public SqlFilter<TEntity> LessThan<TRight>(SqlField<TRight, TFieldType> field)
            => _sqlFilterBuilder.ComparisonFilter<SqlFilter<TEntity>>(LESS_THAN_OPERATOR, field);

        public SqlFilter<TEntity> LessThan(ISqlField field)
            => _sqlFilterBuilder.ComparisonFilter<SqlFilter<TEntity>>(LESS_THAN_OPERATOR, field);

        //----------------------------------------------------------------------------

        private const string LESS_THAN_OR_EQUAL_OPERATOR = "<=";

        public TResult LessThanOrEqual(TFieldType value)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(LESS_THAN_OR_EQUAL_OPERATOR, value);

        public TResult LessThanOrEqual(Expression<Func<TEntity, TFieldType>> field, SqlAlias<TEntity> alias = null)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(LESS_THAN_OR_EQUAL_OPERATOR, field, alias);

        public TResult LessThanOrEqual(SqlField<TEntity, TFieldType> field)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(LESS_THAN_OR_EQUAL_OPERATOR, field);

        public SqlFilter<TEntity> LessThanOrEqual<TRight>(Expression<Func<TRight, TFieldType>> field, SqlAlias<TRight> alias = null)
            => _sqlFilterBuilder.ComparisonFilter<SqlFilter<TEntity>, TRight>(LESS_THAN_OR_EQUAL_OPERATOR, field, alias);

        public SqlFilter<TEntity> LessThanOrEqual<TRight>(SqlField<TRight, TFieldType> field)
            => _sqlFilterBuilder.ComparisonFilter<SqlFilter<TEntity>>(LESS_THAN_OR_EQUAL_OPERATOR, field);

        public SqlFilter<TEntity> LessThanOrEqual(ISqlField field)
            => _sqlFilterBuilder.ComparisonFilter<SqlFilter<TEntity>>(LESS_THAN_OR_EQUAL_OPERATOR, field);

        //----------------------------------------------------------------------------

        public TResult In(params TFieldType[] values)
        {
            Guard.IsNotNull(values);
            Guard.IsPositive(values.Length);
            return _sqlFilterBuilder.ContainsFilter<TResult, TFieldType>("IN", values);
        }

        public TResult In(IEnumerable<TFieldType> values)
        {
            Guard.IsNotNull(values);
            if (!values.Any())
                throw new ArgumentException("Collection is empty");

            return _sqlFilterBuilder.ContainsFilter<TResult, TFieldType>("IN", values);
        }

        //----------------------------------------------------------------------------

        public TResult NotIn(params TFieldType[] values)
        {
            Guard.IsNotNull(values);
            Guard.IsPositive(values.Length);
            return _sqlFilterBuilder.ContainsFilter<TResult, TFieldType>("NOT IN", values);
        }

        public TResult NotIn(IEnumerable<TFieldType> values)
        {
            Guard.IsNotNull(values);
            if (!values.Any())
                throw new ArgumentException("Collection is empty");

            return _sqlFilterBuilder.ContainsFilter<TResult, TFieldType>("NOT IN", values);
        }
    }
}

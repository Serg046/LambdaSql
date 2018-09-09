using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using LambdaSql.Field;
using LambdaSql.Filter.SqlFilterItem;

namespace LambdaSql.Filter
{
    public class SqlFilterField<TEntity, TFieldType, TResult>
        where TResult : SqlFilterBase
    {
        private readonly ISqlField _sqlField;
        private readonly SqlFilterBuilder<TEntity> _sqlFilterBuilder;

        internal SqlFilterField(ImmutableList<SqlFilterItemCallback> sqlFilterItems, ISqlField sqlField,
            Func<ImmutableList<SqlFilterItemCallback>, TResult> filterCreatorFunc)
        {
            _sqlField = sqlField;
            _sqlFilterBuilder = new SqlFilterBuilder<TEntity>(sqlFilterItems, sqlField, filterCreatorFunc);
        }

        //----------------------------------------------------------------------------

        public TResult SatisfyLambda(Func<ISqlField, string> filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));
            return _sqlFilterBuilder.BuildFilter<TResult>(filter(_sqlField));
        }

        //----------------------------------------------------------------------------

        public TResult IsNull() => _sqlFilterBuilder.BuildFilter<TResult>("{0} IS NULL", _sqlField);

        public TResult IsNotNull() => _sqlFilterBuilder.BuildFilter<TResult>("{0} IS NOT NULL", _sqlField);

        //----------------------------------------------------------------------------

        public TResult Like(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException(nameof(value));
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

        public TResult EqualTo<TRight>(Expression<Func<TRight, TFieldType>> field, SqlAlias<TRight> alias = null)
            => _sqlFilterBuilder.ComparisonFilter<TResult, TRight>(EQUAL_OPERATOR, field, alias);

        public TResult EqualTo<TRight>(SqlField<TRight, TFieldType> field)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(EQUAL_OPERATOR, field);

        public TResult EqualTo(ISqlField field)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(EQUAL_OPERATOR, field);


        //----------------------------------------------------------------------------

        private const string NOT_EQUAL_OPERATOR = "<>";

        public TResult NotEqualTo(TFieldType value)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(NOT_EQUAL_OPERATOR, value);

        public TResult NotEqualTo(Expression<Func<TEntity, TFieldType>> field, SqlAlias<TEntity> alias = null)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(NOT_EQUAL_OPERATOR, field, alias);

        public TResult NotEqualTo(SqlField<TEntity, TFieldType> field)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(NOT_EQUAL_OPERATOR, field);

        public TResult NotEqualTo<TRight>(Expression<Func<TRight, TFieldType>> field, SqlAlias<TRight> alias = null)
            => _sqlFilterBuilder.ComparisonFilter<TResult, TRight>(NOT_EQUAL_OPERATOR, field, alias);

        public TResult NotEqualTo<TRight>(SqlField<TRight, TFieldType> field)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(NOT_EQUAL_OPERATOR, field);

        public TResult NotEqualTo(ISqlField field)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(NOT_EQUAL_OPERATOR, field);

        //----------------------------------------------------------------------------
        
        private const string GREATER_THAN_OPERATOR = ">";

        public TResult GreaterThan(TFieldType value)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(GREATER_THAN_OPERATOR, value);

        public TResult GreaterThan(Expression<Func<TEntity, TFieldType>> field, SqlAlias<TEntity> alias = null)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(GREATER_THAN_OPERATOR, field, alias);

        public TResult GreaterThan(SqlField<TEntity, TFieldType> field)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(GREATER_THAN_OPERATOR, field);

        public TResult GreaterThan<TRight>(Expression<Func<TRight, TFieldType>> field, SqlAlias<TRight> alias = null)
            => _sqlFilterBuilder.ComparisonFilter<TResult, TRight>(GREATER_THAN_OPERATOR, field, alias);

        public TResult GreaterThan<TRight>(SqlField<TRight, TFieldType> field)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(GREATER_THAN_OPERATOR, field);

        public TResult GreaterThan(ISqlField field)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(GREATER_THAN_OPERATOR, field);

        //----------------------------------------------------------------------------

        private const string GREATER_THAN_OR_EQUAL_OPERATOR = ">=";

        public TResult GreaterThanOrEqual(TFieldType value)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(GREATER_THAN_OR_EQUAL_OPERATOR, value);

        public TResult GreaterThanOrEqual(Expression<Func<TEntity, TFieldType>> field, SqlAlias<TEntity> alias = null)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(GREATER_THAN_OR_EQUAL_OPERATOR, field, alias);

        public TResult GreaterThanOrEqual(SqlField<TEntity, TFieldType> field)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(GREATER_THAN_OR_EQUAL_OPERATOR, field);

        public TResult GreaterThanOrEqual<TRight>(Expression<Func<TRight, TFieldType>> field, SqlAlias<TRight> alias = null)
            => _sqlFilterBuilder.ComparisonFilter<TResult, TRight>(GREATER_THAN_OR_EQUAL_OPERATOR, field, alias);

        public TResult GreaterThanOrEqual<TRight>(SqlField<TRight, TFieldType> field)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(GREATER_THAN_OR_EQUAL_OPERATOR, field);

        public TResult GreaterThanOrEqual(ISqlField field)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(GREATER_THAN_OR_EQUAL_OPERATOR, field);

        //----------------------------------------------------------------------------

        private const string LESS_THAN_OPERATOR = "<";

        public TResult LessThan(TFieldType value)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(LESS_THAN_OPERATOR, value);

        public TResult LessThan(Expression<Func<TEntity, TFieldType>> field, SqlAlias<TEntity> alias = null)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(LESS_THAN_OPERATOR, field, alias);

        public TResult LessThan(SqlField<TEntity, TFieldType> field)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(LESS_THAN_OPERATOR, field);

        public TResult LessThan<TRight>(Expression<Func<TRight, TFieldType>> field, SqlAlias<TRight> alias = null)
            => _sqlFilterBuilder.ComparisonFilter<TResult, TRight>(LESS_THAN_OPERATOR, field, alias);

        public TResult LessThan<TRight>(SqlField<TRight, TFieldType> field)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(LESS_THAN_OPERATOR, field);

        public TResult LessThan(ISqlField field)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(LESS_THAN_OPERATOR, field);

        //----------------------------------------------------------------------------

        private const string LESS_THAN_OR_EQUAL_OPERATOR = "<=";

        public TResult LessThanOrEqual(TFieldType value)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(LESS_THAN_OR_EQUAL_OPERATOR, value);

        public TResult LessThanOrEqual(Expression<Func<TEntity, TFieldType>> field, SqlAlias<TEntity> alias = null)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(LESS_THAN_OR_EQUAL_OPERATOR, field, alias);

        public TResult LessThanOrEqual(SqlField<TEntity, TFieldType> field)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(LESS_THAN_OR_EQUAL_OPERATOR, field);

        public TResult LessThanOrEqual<TRight>(Expression<Func<TRight, TFieldType>> field, SqlAlias<TRight> alias = null)
            => _sqlFilterBuilder.ComparisonFilter<TResult, TRight>(LESS_THAN_OR_EQUAL_OPERATOR, field, alias);

        public TResult LessThanOrEqual<TRight>(SqlField<TRight, TFieldType> field)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(LESS_THAN_OR_EQUAL_OPERATOR, field);

        public TResult LessThanOrEqual(ISqlField field)
            => _sqlFilterBuilder.ComparisonFilter<TResult>(LESS_THAN_OR_EQUAL_OPERATOR, field);

        //----------------------------------------------------------------------------

        public TResult In(params TFieldType[] values)
        {
            if (values?.Any() != true) throw new ArgumentException("Sequence contains no elements");
            return _sqlFilterBuilder.ContainsFilter<TResult, TFieldType>("IN", values);
        }

        public TResult In(IEnumerable<TFieldType> values)
        {
            if (values?.Any() != true) throw new ArgumentException("Sequence contains no elements");
            return _sqlFilterBuilder.ContainsFilter<TResult, TFieldType>("IN", values);
        }

        //----------------------------------------------------------------------------

        public TResult NotIn(params TFieldType[] values)
        {
            if (values?.Any() != true) throw new ArgumentException("Sequence contains no elements");
            return _sqlFilterBuilder.ContainsFilter<TResult, TFieldType>("NOT IN", values);
        }

        public TResult NotIn(IEnumerable<TFieldType> values)
        {
            if (values?.Any() != true) throw new ArgumentException("Sequence contains no elements");
            return _sqlFilterBuilder.ContainsFilter<TResult, TFieldType>("NOT IN", values);
        }
    }
}

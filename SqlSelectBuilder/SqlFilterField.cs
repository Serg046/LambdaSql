using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using GuardExtensions;

namespace SqlSelectBuilder
{
    public class SqlFilterField<TEntity, TType>
    {
        private readonly string _filter;
        private readonly string _field;

        internal SqlFilterField(string field)
        {
            Contract.Requires(field.IsNotEmpty());
            _filter = string.Empty;
            _field = field;
        }

        internal SqlFilterField(string filter, string field)
        {
            Guard.IsNotEmpty(filter);
            Guard.IsNotEmpty(field);
            _filter = filter;
            _field = field;
        }

        public SqlFilter<TEntity> SatisfyLambda(Func<string, IParameterConverter, string> filter)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(filter);
            return new SqlFilter<TEntity>(_filter + filter(_field, MetadataProvider.Instance));
        }

        public SqlFilter<TEntity> IsNull()
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            return new SqlFilter<TEntity>(_filter + _field + " IS NULL");
        }

        public SqlFilter<TEntity> IsNotNull()
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            return new SqlFilter<TEntity>(_filter + _field + " IS NOT NULL");
        }

        public SqlFilter<TEntity> Like(string value)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotEmpty(value);
            return new SqlFilter<TEntity>($"{_filter}{_field} LIKE {MetadataProvider.Instance.ParameterToString(value)}");
        }

        //----------------------------------------------------------------------------

        private SqlFilter<TEntity> LogicFilter(string logicOperator, string value)
        {
            Contract.Requires(logicOperator.IsNotEmpty());
            Contract.Requires(value.IsNotEmpty());
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            return new SqlFilter<TEntity>($"{_filter}{_field} {logicOperator} {value}");
        }

        //----------------------------------------------------------------------------

        public SqlFilter<TEntity> EqualTo(TType value)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            return LogicFilter("=", MetadataProvider.Instance.ParameterToString(value));
        }

        public SqlFilter<TEntity> EqualTo(ISqlField field)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            return LogicFilter("=", field.ShortView);
        }

        public SqlFilter<TEntity> EqualTo(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            if (alias == null)
                alias = MetadataProvider.Instance.AliasFor<TEntity>();
            return EqualTo<TEntity>(field, alias);
        }

        public SqlFilter<TEntity> EqualTo<TRight>(Expression<Func<TRight, TType>> field, SqlAlias<TRight> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            if (alias == null)
                alias = MetadataProvider.Instance.AliasFor<TRight>();
            return LogicFilter("=", alias.Value + "." + MetadataProvider.Instance.GetPropertyName(field));
        }

        //----------------------------------------------------------------------------

        public SqlFilter<TEntity> NotEqualTo(TType value)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            return LogicFilter("<>", MetadataProvider.Instance.ParameterToString(value));
        }

        public SqlFilter<TEntity> NotEqualTo(ISqlField field)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            return LogicFilter("<>", field.ShortView);
        }

        public SqlFilter<TEntity> NotEqualTo(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            if (alias == null)
                alias = MetadataProvider.Instance.AliasFor<TEntity>();
            return NotEqualTo<TEntity>(field, alias);
        }

        public SqlFilter<TEntity> NotEqualTo<TRight>(Expression<Func<TRight, TType>> field, SqlAlias<TRight> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            if (alias == null)
                alias = MetadataProvider.Instance.AliasFor<TRight>();
            return LogicFilter("<>", alias.Value + "." + MetadataProvider.Instance.GetPropertyName(field));
        }

        //----------------------------------------------------------------------------

        public SqlFilter<TEntity> GreaterThan(TType value)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            return LogicFilter(">", MetadataProvider.Instance.ParameterToString(value));
        }

        public SqlFilter<TEntity> GreaterThan(ISqlField field)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            return LogicFilter(">", field.ShortView);
        }

        public SqlFilter<TEntity> GreaterThan(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            if (alias == null)
                alias = MetadataProvider.Instance.AliasFor<TEntity>();
            return GreaterThan<TEntity>(field, alias);
        }

        public SqlFilter<TEntity> GreaterThan<TRight>(Expression<Func<TRight, TType>> field, SqlAlias<TRight> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            if (alias == null)
                alias = MetadataProvider.Instance.AliasFor<TRight>();
            return LogicFilter(">", alias.Value + "." + MetadataProvider.Instance.GetPropertyName(field));
        }

        //----------------------------------------------------------------------------

        public SqlFilter<TEntity> GreaterThanOrEqual(TType value)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            return LogicFilter(">=", MetadataProvider.Instance.ParameterToString(value));
        }

        public SqlFilter<TEntity> GreaterThanOrEqual(ISqlField field)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            return LogicFilter(">=", field.ShortView);
        }

        public SqlFilter<TEntity> GreaterThanOrEqual(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            if (alias == null)
                alias = MetadataProvider.Instance.AliasFor<TEntity>();
            return GreaterThanOrEqual<TEntity>(field, alias);
        }

        public SqlFilter<TEntity> GreaterThanOrEqual<TRight>(Expression<Func<TRight, TType>> field, SqlAlias<TRight> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            if (alias == null)
                alias = MetadataProvider.Instance.AliasFor<TRight>();
            return LogicFilter(">=", alias.Value + "." + MetadataProvider.Instance.GetPropertyName(field));
        }

        //----------------------------------------------------------------------------

        public SqlFilter<TEntity> LessThan(TType value)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            return LogicFilter("<", MetadataProvider.Instance.ParameterToString(value));
        }

        public SqlFilter<TEntity> LessThan(ISqlField field)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            return LogicFilter("<", field.ShortView);
        }

        public SqlFilter<TEntity> LessThan(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            if (alias == null)
                alias = MetadataProvider.Instance.AliasFor<TEntity>();
            return LessThan<TEntity>(field, alias);
        }

        public SqlFilter<TEntity> LessThan<TRight>(Expression<Func<TRight, TType>> field, SqlAlias<TRight> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            if (alias == null)
                alias = MetadataProvider.Instance.AliasFor<TRight>();
            return LogicFilter("<", alias.Value + "." + MetadataProvider.Instance.GetPropertyName(field));
        }

        //----------------------------------------------------------------------------

        public SqlFilter<TEntity> LessThanOrEqual(TType value)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            return LogicFilter("<=", MetadataProvider.Instance.ParameterToString(value));
        }

        public SqlFilter<TEntity> LessThanOrEqual(ISqlField field)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            return LogicFilter("<=", field.ShortView);
        }

        public SqlFilter<TEntity> LessThanOrEqual(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            if (alias == null)
                alias = MetadataProvider.Instance.AliasFor<TEntity>();
            return LessThanOrEqual<TEntity>(field, alias);
        }

        public SqlFilter<TEntity> LessThanOrEqual<TRight>(Expression<Func<TRight, TType>> field, SqlAlias<TRight> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(field);
            if (alias == null)
                alias = MetadataProvider.Instance.AliasFor<TRight>();
            return LogicFilter("<=", alias.Value + "." + MetadataProvider.Instance.GetPropertyName(field));
        }

        //----------------------------------------------------------------------------

        public SqlFilter<TEntity> In(params TType[] values)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(values);
            Guard.IsPositive(values.Length);
            return In((IEnumerable<TType>)values);
        }

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public SqlFilter<TEntity> In(IEnumerable<TType> values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (!values.Any())
                throw new ArgumentException("Collection is empty");
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Contract.EndContractBlock();
            var parameters = string.Join(",", values.Select(v => MetadataProvider.Instance.ParameterToString(v)));
            return new SqlFilter<TEntity>($"{_filter}{_field} IN ({parameters})");
        }

        //----------------------------------------------------------------------------
    }
}

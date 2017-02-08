using System;
using System.Collections.Immutable;
using System.Linq.Expressions;

namespace LambdaSqlBuilder.SqlFilter.SqlFilterField
{
    public class SqlFilterField<TEntity, TType> : RestrictedSqlFilterField<TEntity, TType, SqlFilter<TEntity>>
    {
        internal SqlFilterField(ImmutableList<SqlFilterItemFunc> sqlFilterItems, ISqlField sqlField)
            : base(sqlFilterItems, sqlField)
        {
        }

        internal override SqlFilter<TEntity> BuildFilter(ImmutableList<SqlFilterItemFunc> sqlFilterItems)
            => new SqlFilter<TEntity>(sqlFilterItems);

        //----------------------------------------------------------------------------

        public SqlFilter<TEntity> EqualTo(ISqlField field) => LogicFilter("=", field);

        public SqlFilter<TEntity> EqualTo<TRight>(Expression<Func<TRight, TType>> field, SqlAlias<TRight> alias = null)
        {
            alias = CheckAlias(alias);
            return LogicFilter("=", field, alias);
        }

        //----------------------------------------------------------------------------

        public SqlFilter<TEntity> NotEqualTo(ISqlField field) => LogicFilter("<>", field);

        public SqlFilter<TEntity> NotEqualTo<TRight>(Expression<Func<TRight, TType>> field, SqlAlias<TRight> alias = null)
        {
            alias = CheckAlias(alias);
            return LogicFilter("<>", field, alias);
        }

        //----------------------------------------------------------------------------

        public SqlFilter<TEntity> GreaterThan(ISqlField field) => LogicFilter(">", field);

        public SqlFilter<TEntity> GreaterThan<TRight>(Expression<Func<TRight, TType>> field, SqlAlias<TRight> alias = null)
        {
            alias = CheckAlias(alias);
            return LogicFilter(">", field, alias);
        }

        //----------------------------------------------------------------------------

        public SqlFilter<TEntity> GreaterThanOrEqual(ISqlField field) => LogicFilter(">=", field);

        public SqlFilter<TEntity> GreaterThanOrEqual<TRight>(Expression<Func<TRight, TType>> field, SqlAlias<TRight> alias = null)
        {
            alias = CheckAlias(alias);
            return LogicFilter(">=", field, alias);
        }

        //----------------------------------------------------------------------------

        public SqlFilter<TEntity> LessThan(ISqlField field) => LogicFilter("<", field);

        public SqlFilter<TEntity> LessThan<TRight>(Expression<Func<TRight, TType>> field, SqlAlias<TRight> alias = null)
        {
            alias = CheckAlias(alias);
            return LogicFilter("<", field, alias);
        }

        //----------------------------------------------------------------------------

        public SqlFilter<TEntity> LessThanOrEqual(ISqlField field) => LogicFilter("<=", field);

        public SqlFilter<TEntity> LessThanOrEqual<TRight>(Expression<Func<TRight, TType>> field, SqlAlias<TRight> alias = null)
        {
            alias = CheckAlias(alias);
            return LogicFilter("<=", field, alias);
        }
    }
}

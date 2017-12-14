using System;
using System.Data.Common;
using System.Linq.Expressions;
using LambdaSql.Field;
using LambdaSql.Filter;
using LambdaSql.QueryBuilder;

namespace LambdaSql
{
    public interface ISqlSelect
    {
        string RawSql { get; }
        string ParametricSql { get; }
        SqlSelectInfo Info { get; }
        Type EntityType { get; }
        DbParameter[] Parameters { get; }


        ISqlSelect Extend(Func<ISqlSelectQueryBuilder, ISqlSelectQueryBuilder> decorationCallback);
        ISqlSelect Distinct(bool isDistinct);

        ISqlSelect AddFields(params ISqlField[] fields);
        ISqlSelect AddFields<TEntity>(SqlAlias<TEntity> alias = null,
            params Expression<Func<TEntity, object>>[] fields);

        ISqlSelect GroupBy(params ISqlField[] fields);
        ISqlSelect GroupBy<TEntity>(SqlAlias<TEntity> alias = null,
            params Expression<Func<TEntity, object>>[] fields);

        ISqlSelect OrderBy(params ISqlField[] fields);
        ISqlSelect OrderBy<TEntity>(SqlAlias<TEntity> alias = null,
            params Expression<Func<TEntity, object>>[] fields);

        ISqlSelect Join<TJoin>(JoinType joinType, ISqlFilter condition, SqlAlias<TJoin> joinAlias = null);

        ISqlSelect Where(ISqlFilter filter);
        ISqlSelect Having(ISqlFilter filter);
    }
}

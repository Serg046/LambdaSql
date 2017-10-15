using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using LambdaSql.Field;
using LambdaSql.Filter;

namespace LambdaSql
{
    public interface ISqlSelectInfo
    {
        IEnumerable<ISqlField> SelectFields { get; }
        IEnumerable<ISqlField> GroupByFields { get; }
        IEnumerable<ISqlField> OrderByFields { get; }
        IEnumerable<ISqlAlias> TableAliases { get; }
        ISqlFilter WhereFilter { get; }
        ISqlFilter HavingFilter { get; }
    }

    public interface ISqlSelect : ISqlSelectInfo
    {
        string CommandText { get; }
        Type EntityType { get; }
        ISqlSelect Top(int top);
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

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SqlSelectBuilder.SqlFilter;

namespace SqlSelectBuilder
{
    public interface ISqlSelect
    {
        Type EntityType { get; }
        string CommandText { get; }
        IEnumerable<ISqlField> SelectFields { get; }
        IEnumerable<ISqlField> GroupByFields { get; }
        IEnumerable<ISqlField> OrderByFields { get; }
        IEnumerable<ISqlAlias> Aliases { get; }

        void Top(int top, bool topByPercent = false);
        void Distinct(bool isDistinct);

        void AddField(ISqlField field);
        void AddFields<TEntity>(SqlAlias<TEntity> alias = null,
            params Expression<Func<TEntity, object>>[] fields);

        void GroupBy<TEntity>(SqlAlias<TEntity> alias = null,
            params Expression<Func<TEntity, object>>[] fields);

        void OrderBy<TEntity>(SqlAlias<TEntity> alias = null,
            params Expression<Func<TEntity, object>>[] fields);

        void Join<TJoin>(JoinType joinType, ISqlFilter condition, SqlAlias<TJoin> joinAlias = null);

        void Where(ISqlFilter filter);
        void Having(ISqlFilter filter);
    }
}

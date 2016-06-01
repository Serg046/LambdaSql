using System;
using GuardExtensions;

namespace SqlSelectBuilder
{
    public interface ISqlJoin
    {
        JoinType JoinType { get; }
        Type JoinEntityType { get; }
        ISqlAlias JoinAlias { get; }
        ISqlFilter JoinCondition { get; }
    }

    public enum JoinType
    {
        Inner,
        Left,
        Right,
        Full
    }

    public class SqlJoin<TJoin> : ISqlJoin
    {
        public SqlJoin(JoinType joinType, ISqlFilter joinCondition, SqlAlias<TJoin> joinAlias)
        {
            Guard.IsNotNull(joinCondition);
            Guard.IsNotNull(joinAlias);

            JoinType = joinType;
            JoinCondition = joinCondition;
            JoinAlias = joinAlias;
        }

        public JoinType JoinType { get; }
        public Type JoinEntityType => typeof (TJoin);
        public SqlAlias<TJoin> JoinAlias { get; } 
        ISqlAlias ISqlJoin.JoinAlias => JoinAlias;
        public ISqlFilter JoinCondition { get; }

        public override string ToString()
        {
            var entity = MetadataProvider.Instance.GetTableName(JoinEntityType) + " " + JoinAlias.Value;
            return $"{JoinType.ToString().ToUpper()} JOIN\r\n    {entity} ON {JoinCondition.Filter }";
        }
    }
}

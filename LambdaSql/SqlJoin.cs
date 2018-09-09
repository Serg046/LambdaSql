using System;
using LambdaSql.Filter;

namespace LambdaSql
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
            JoinType = joinType;
            JoinCondition = joinCondition ?? throw new ArgumentNullException(nameof(joinCondition));
            JoinAlias = joinAlias ?? throw new ArgumentNullException(nameof(joinAlias));
        }

        public JoinType JoinType { get; }
        public Type JoinEntityType => typeof (TJoin);
        public SqlAlias<TJoin> JoinAlias { get; } 
        ISqlAlias ISqlJoin.JoinAlias => JoinAlias;
        public ISqlFilter JoinCondition { get; }

        public override string ToString()
        {
            var entity = MetadataProvider.Instance.GetTableName(JoinEntityType) + " " + JoinAlias.Value;
            return $"{JoinType.ToString().ToUpper()} JOIN\r\n    {entity} ON {JoinCondition.RawSql }";
        }
    }
}

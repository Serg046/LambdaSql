using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using GuardExtensions;

namespace SqlSelectBuilder
{
    [ContractClass(typeof(ISqlJoinContract))]
    public interface ISqlJoin
    {
        JoinType JoinType { get; }
        Type JoinEntityType { get; }
        ISqlAlias JoinAlias { get; }
        ISqlFilter JoinCondition { get; }
    }

    [ContractClassFor(typeof(ISqlJoin))]
    [ExcludeFromCodeCoverage]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
    internal abstract class ISqlJoinContract : ISqlJoin
    {
        public ISqlAlias JoinAlias
        {
            get
            {
                Contract.Ensures(Contract.Result<ISqlAlias>() != null);
                throw new NotImplementedException();
            }
        }

        public ISqlFilter JoinCondition
        {
            get
            {
                Contract.Ensures(Contract.Result<ISqlFilter>() != null);
                throw new NotImplementedException();
            }
        }

        public Type JoinEntityType
        {
            get
            {
                Contract.Ensures(Contract.Result<Type>() != null);
                throw new NotImplementedException();
            }
        }

        public JoinType JoinType { get; }
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

using System;

namespace LambdaSql.QueryBuilder
{
    public interface ISqlSelectQueryBuilder : ICloneable
    {
        ISqlSelectQueryBuilder ModifySelectFields(ModifySelectFieldsCallback modificationCallback);
        string Build(SqlSelectInfo info, bool parametric);
    }
}

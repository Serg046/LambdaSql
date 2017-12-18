using System;

namespace LambdaSql.QueryBuilder
{
    public interface ISqlSelectQueryBuilder : ICloneable
    {
        string Build(SqlSelectInfo info, bool parametric);

        ISqlSelectQueryBuilder ModifySelectFields(ModifyQueryPartCallback modificationCallback);
        ISqlSelectQueryBuilder ModifyJoins(ModifyQueryPartCallback modificationCallback);
        ISqlSelectQueryBuilder ModifyWhereFilters(ModifyQueryPartCallback modificationCallback);
        ISqlSelectQueryBuilder ModifyGroupByFields(ModifyQueryPartCallback modificationCallback);
        ISqlSelectQueryBuilder ModifyHavingFilters(ModifyQueryPartCallback modificationCallback);
        ISqlSelectQueryBuilder ModifyOrderByFields(ModifyQueryPartCallback modificationCallback);
        ISqlSelectQueryBuilder Modify(ModifyQueryPartCallback modificationCallback);
    }
}

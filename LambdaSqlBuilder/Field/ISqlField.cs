using System;

namespace LambdaSqlBuilder.Field
{
    public interface ISqlField
    {
        ISqlAlias Alias { get; set; }
        Type EntityType { get; }
        string Name { get; }
        string AsAlias { get; set; }
        string ShortView { get; }
        string View { get; }
        AggregateFunc? Aggregation { get; }
        ISqlField Clone();
    }
}

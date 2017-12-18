using System;

namespace LambdaSql.Field
{
    public interface ISqlField
    {
        ISqlAlias Alias { get; set; }
        Type EntityType { get; }
        string Name { get; }
        string AsAlias { get; set; }
        string ShortString { get; }
        string String { get; }
        AggregateFunc? Aggregation { get; }
        ISqlField Clone();
    }
}

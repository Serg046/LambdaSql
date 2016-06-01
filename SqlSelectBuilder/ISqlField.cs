using System;

namespace SqlSelectBuilder
{
    public interface ISqlField
    {
        ISqlAlias Alias { get; set; }
        Type EntityType { get; }
        string Name { get; set; }
        string AsAlias { get; set; }
        string ShortView { get; }
        string View { get; }
        FieldAggregation? Aggregation { get; set; }
    }
}

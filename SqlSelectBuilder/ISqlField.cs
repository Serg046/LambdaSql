namespace SqlSelectBuilder
{
    public interface ISqlField
    {
        ISqlAlias Alias { get; set; }
        string Name { get; set; }
        string AsAlias { get; set; }
        string ShortView { get; }
        string View { get; }
        FieldAggregation? Aggregation { get; set; }
    }
}

namespace LambdaSql
{
    public static class MsSqlServerExtensions
    {
        public static SqlSelect<T> Top<T>(this SqlSelect<T> sqlSelect, int top)
            => sqlSelect.Extend(builder => builder.ModifySelectFields(fields => $"TOP {top} {fields}"));

        public static SqlSelect Top(this SqlSelect sqlSelect, int top)
            => sqlSelect.Extend(builder => builder.ModifySelectFields(fields => $"TOP {top} {fields}"));

        public static ISqlSelect Top(this ISqlSelect sqlSelect, int top)
            => sqlSelect.Extend(builder => builder.ModifySelectFields(fields => $"TOP {top} {fields}"));
    }
}

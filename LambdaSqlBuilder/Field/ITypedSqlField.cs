using System;

namespace LambdaSqlBuilder.Field
{
    public interface ITypedSqlField : ISqlField
    {
        Type FieldType { get; }
    }
}

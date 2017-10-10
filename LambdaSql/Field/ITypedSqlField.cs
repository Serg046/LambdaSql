using System;

namespace LambdaSql.Field
{
    public interface ITypedSqlField : ISqlField
    {
        Type FieldType { get; }
    }
}

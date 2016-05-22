using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using GuardExtensions;

namespace SqlSelectBuilder
{
    [ContractClass(typeof (ISqlAliasContract))]
    public interface ISqlAlias
    {
        string Value { get; }
        Type EntityType { get; }
    }

    [ContractClassFor(typeof (ISqlAlias))]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
    internal abstract class ISqlAliasContract : ISqlAlias
    {
        public string Value
        {
            get
            {
                Contract.Ensures(Contract.Result<string>().IsNotEmpty());
                throw new NotImplementedException();
            }
        }

        public Type EntityType
        {
            get
            {
                Contract.Ensures(Contract.Result<Type>() != null);
                throw new NotImplementedException();
            }
        }
    }


    public class SqlAlias<T> : ISqlAlias
    {
        public SqlAlias(string aliasName)
        {
            Guard.IsNotEmpty(aliasName);
            Value = aliasName;
        }

        public string Value { get; }

        public Type EntityType => typeof (T);

        public override string ToString() => Value;

        public override bool Equals(object obj)
        {
            var alias = obj as SqlAlias<T>;
            return alias != null && Equals(alias);
        }

        public bool Equals(SqlAlias<T> obj)
        {
            return obj != null && string.Equals(Value, obj.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}

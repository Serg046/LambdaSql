using System;

namespace LambdaSql
{
    public interface ISqlAlias
    {
        string Value { get; }
        Type EntityType { get; }
    }

    public class SqlAlias<T> : ISqlAlias
    {
        public SqlAlias(string aliasName)
        {
            if (string.IsNullOrWhiteSpace(aliasName)) throw new ArgumentException(nameof(aliasName));
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

    public class SqlAlias : ISqlAlias
    {
        public SqlAlias(string aliasName)
        {
            if (string.IsNullOrWhiteSpace(aliasName)) throw new ArgumentException(nameof(aliasName));
            Value = aliasName;
        }

        public Type EntityType => null;

        public string Value { get; }

        public override string ToString() => Value;

        public override bool Equals(object obj)
        {
            var alias = obj as SqlAlias;
            return alias != null && Equals(alias);
        }

        public bool Equals(SqlAlias obj)
        {
            return obj != null && string.Equals(Value, obj.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}

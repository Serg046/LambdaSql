using System;
using System.Linq.Expressions;
using System.Text;

namespace LambdaSql.Field
{
    public class SqlField : ISqlField
    {
        internal SqlField(Type entityType)
        {
            EntityType = entityType;
        }

        public ISqlAlias Alias { get; set; }
        public Type EntityType { get; }
        public string Name { get; internal set; }
        public string AsAlias { get; set; }
        public AggregateFunc? Aggregation { get; internal set; }

        public string ShortString => GetShortStringBuilder().ToString();

        public string String
        {
            get
            {
                var sb = GetShortStringBuilder();
                if (AsAlias != null)
                    sb.Append(" AS ").Append(AsAlias);
                return sb.ToString();
            }
        }

        private StringBuilder GetShortStringBuilder()
        {
            var sb = Alias != null && Alias.Value.IsNotEmpty()
                ? new StringBuilder(Alias.Value + "." + Name)
                : new StringBuilder(Name);
            if (Aggregation != null)
                sb.Insert(0, Aggregation.ToString().ToUpper() + "(").Append(")");
            return sb;
        }

        public override string ToString() => String;

        public ISqlField Clone() => (ISqlField)MemberwiseClone();

        public static SqlField From(Type entityType, ISqlAlias alias, string name, string asAlias = null)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return new SqlField(entityType)
            {
                Alias = alias,
                Name = name,
                AsAlias = asAlias
            };
        }

        public static TypedSqlField From(Type entityType, Type fieldType, ISqlAlias alias, string name, string asAlias = null)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return new TypedSqlField(entityType, fieldType)
            {
                Alias = alias,
                Name = name,
                AsAlias = asAlias
            };
        }
    }

    public class TypedSqlField : SqlField, ITypedSqlField
    {
        internal TypedSqlField(Type entityType, Type fieldType) : base(entityType)
        {
            FieldType = fieldType;
        }

        public Type FieldType { get; }
    }

    public class SqlField<TEntity, TFieldType> : TypedSqlField
    {
        internal SqlField() : base(typeof(TEntity), typeof(TFieldType))
        {
        }
    }

    public class SqlField<TEntity> : SqlField
    {
        internal SqlField() : base(typeof(TEntity))
        {
        }

        private static SqlField<TEntity, TFieldType> Aggregate<TFieldType>(ISqlAlias alias,
            LambdaExpression field, AggregateFunc? aggregation, string asAlias)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            if (alias == null) throw new ArgumentNullException(nameof(alias));
            var fieldName = MetadataProvider.Instance.GetPropertyName(field);
            return new SqlField<TEntity, TFieldType> { Alias = alias, Name = fieldName, AsAlias = asAlias, Aggregation = aggregation };
        }

        public static SqlField<TEntity, TFieldType> From<TFieldType>(Expression<Func<TEntity, TFieldType>> field, string asAlias = null)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            return From(MetadataProvider.Instance.AliasFor<TEntity>(), field, asAlias);
        }

        public static SqlField<TEntity, TFieldType> From<TFieldType>(SqlAlias<TEntity> alias, Expression<Func<TEntity, TFieldType>> field, string asAlias = null)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            if (alias == null) throw new ArgumentNullException(nameof(alias));
            return Aggregate<TFieldType>(alias, field, null, asAlias);
        }

        public static SqlField<TEntity, int> Min<TFieldType>(Expression<Func<TEntity, TFieldType>> field, string asAlias = null)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            return Min(MetadataProvider.Instance.AliasFor<TEntity>(), field, asAlias);
        }

        public static SqlField<TEntity, int> Min<TFieldType>(SqlAlias<TEntity> alias, Expression<Func<TEntity, TFieldType>> field, string asAlias = null)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            if (alias == null) throw new ArgumentNullException(nameof(alias));
            return Aggregate<int>(alias, field, AggregateFunc.Min, asAlias);
        }

        public static SqlField<TEntity, int> Max<TFieldType>(Expression<Func<TEntity, TFieldType>> field, string asAlias = null)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            return Max(MetadataProvider.Instance.AliasFor<TEntity>(), field, asAlias);
        }

        public static SqlField<TEntity, int> Max<TFieldType>(SqlAlias<TEntity> alias, Expression<Func<TEntity, TFieldType>> field, string asAlias = null)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            if (alias == null) throw new ArgumentNullException(nameof(alias));
            return Aggregate<int>(alias, field, AggregateFunc.Max, asAlias);
        }

        public static SqlField<TEntity, int> Avg<TFieldType>(Expression<Func<TEntity, TFieldType>> field, string asAlias = null)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            return Avg(MetadataProvider.Instance.AliasFor<TEntity>(), field, asAlias);
        }

        public static SqlField<TEntity, int> Avg<TFieldType>(SqlAlias<TEntity> alias, Expression<Func<TEntity, TFieldType>> field, string asAlias = null)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            if (alias == null) throw new ArgumentNullException(nameof(alias));
            return Aggregate<int>(alias, field, AggregateFunc.Avg, asAlias);
        }

        public static SqlField<TEntity, int> Sum<TFieldType>(Expression<Func<TEntity, TFieldType>> field, string asAlias = null)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            return Sum(MetadataProvider.Instance.AliasFor<TEntity>(), field, asAlias);
        }

        public static SqlField<TEntity, int> Sum<TFieldType>(SqlAlias<TEntity> alias, Expression<Func<TEntity, TFieldType>> field, string asAlias = null)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            if (alias == null) throw new ArgumentNullException(nameof(alias));
            return Aggregate<int>(alias, field, AggregateFunc.Sum, asAlias);
        }

        public static SqlField<TEntity, int> Count<TFieldType>(Expression<Func<TEntity, TFieldType>> field, string asAlias = null)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            return Count(MetadataProvider.Instance.AliasFor<TEntity>(), field, asAlias);
        }

        public static SqlField<TEntity, int> Count<TFieldType>(SqlAlias<TEntity> alias, Expression<Func<TEntity, TFieldType>> field, string asAlias = null)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            if (alias == null) throw new ArgumentNullException(nameof(alias));
            return Aggregate<int>(alias, field, AggregateFunc.Count, asAlias);
        }
    }
}

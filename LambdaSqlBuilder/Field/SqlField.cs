using System;
using System.Linq.Expressions;
using System.Text;
using GuardExtensions;

namespace LambdaSqlBuilder.Field
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

        public string ShortView => GetShortViewBuilder().ToString();

        public string View
        {
            get
            {
                var sb = GetShortViewBuilder();
                if (AsAlias != null)
                    sb.Append(" AS ").Append(AsAlias);
                return sb.ToString();
            }
        }

        private StringBuilder GetShortViewBuilder()
        {
            var sb = Alias != null && Alias.Value.IsNotEmpty()
                ? new StringBuilder(Alias.Value + "." + Name)
                : new StringBuilder(Name);
            if (Aggregation != null)
                sb.Insert(0, Aggregation.ToString().ToUpper() + "(").Append(")");
            return sb;
        }

        public override string ToString() => View;

        public ISqlField Clone() => (ISqlField)MemberwiseClone();

        public static SqlField From(Type entityType, ISqlAlias alias, string name, string asAlias = null)
        {
            Guard.IsNotNull(name);
            return new SqlField(entityType)
            {
                Alias = alias,
                Name = name,
                AsAlias = asAlias
            };
        }

        public static TypedSqlField From(Type entityType, Type fieldType, ISqlAlias alias, string name, string asAlias = null)
        {
            Guard.IsNotNull(name);
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
            Guard.IsNotNull(alias);
            Guard.IsNotNull(field);

            var fieldName = MetadataProvider.Instance.GetPropertyName(field);
            return new SqlField<TEntity, TFieldType> { Alias = alias, Name = fieldName, AsAlias = asAlias, Aggregation = aggregation };
        }

        public static SqlField<TEntity, TFieldType> From<TFieldType>(Expression<Func<TEntity, TFieldType>> field, string asAlias = null)
        {
            Guard.IsNotNull(field);
            return From(MetadataProvider.Instance.AliasFor<TEntity>(), field, asAlias);
        }

        public static SqlField<TEntity, TFieldType> From<TFieldType>(SqlAlias<TEntity> alias, Expression<Func<TEntity, TFieldType>> field, string asAlias = null)
        {
            Guard.IsNotNull(alias);
            Guard.IsNotNull(field);
            return Aggregate<TFieldType>(alias, field, null, asAlias);
        }

        public static SqlField<TEntity, int> Min<TFieldType>(Expression<Func<TEntity, TFieldType>> field, string asAlias = null)
        {
            Guard.IsNotNull(field);
            return Min(MetadataProvider.Instance.AliasFor<TEntity>(), field, asAlias);
        }

        public static SqlField<TEntity, int> Min<TFieldType>(SqlAlias<TEntity> alias, Expression<Func<TEntity, TFieldType>> field, string asAlias = null)
        {
            Guard.IsNotNull(alias);
            Guard.IsNotNull(field);
            return Aggregate<int>(alias, field, AggregateFunc.Min, asAlias);
        }

        public static SqlField<TEntity, int> Max<TFieldType>(Expression<Func<TEntity, TFieldType>> field, string asAlias = null)
        {
            Guard.IsNotNull(field);
            return Max(MetadataProvider.Instance.AliasFor<TEntity>(), field, asAlias);
        }

        public static SqlField<TEntity, int> Max<TFieldType>(SqlAlias<TEntity> alias, Expression<Func<TEntity, TFieldType>> field, string asAlias = null)
        {
            Guard.IsNotNull(alias);
            Guard.IsNotNull(field);
            return Aggregate<int>(alias, field, AggregateFunc.Max, asAlias);
        }

        public static SqlField<TEntity, int> Avg<TFieldType>(Expression<Func<TEntity, TFieldType>> field, string asAlias = null)
        {
            Guard.IsNotNull(field);
            return Avg(MetadataProvider.Instance.AliasFor<TEntity>(), field, asAlias);
        }

        public static SqlField<TEntity, int> Avg<TFieldType>(SqlAlias<TEntity> alias, Expression<Func<TEntity, TFieldType>> field, string asAlias = null)
        {
            Guard.IsNotNull(alias);
            Guard.IsNotNull(field);
            return Aggregate<int>(alias, field, AggregateFunc.Avg, asAlias);
        }

        public static SqlField<TEntity, int> Sum<TFieldType>(Expression<Func<TEntity, TFieldType>> field, string asAlias = null)
        {
            Guard.IsNotNull(field);
            return Sum(MetadataProvider.Instance.AliasFor<TEntity>(), field, asAlias);
        }

        public static SqlField<TEntity, int> Sum<TFieldType>(SqlAlias<TEntity> alias, Expression<Func<TEntity, TFieldType>> field, string asAlias = null)
        {
            Guard.IsNotNull(alias);
            Guard.IsNotNull(field);
            return Aggregate<int>(alias, field, AggregateFunc.Sum, asAlias);
        }

        public static SqlField<TEntity, int> Count<TFieldType>(Expression<Func<TEntity, TFieldType>> field, string asAlias = null)
        {
            Guard.IsNotNull(field);
            return Count(MetadataProvider.Instance.AliasFor<TEntity>(), field, asAlias);
        }

        public static SqlField<TEntity, int> Count<TFieldType>(SqlAlias<TEntity> alias, Expression<Func<TEntity, TFieldType>> field, string asAlias = null)
        {
            Guard.IsNotNull(alias);
            Guard.IsNotNull(field);
            return Aggregate<int>(alias, field, AggregateFunc.Count, asAlias);
        }
    }
}

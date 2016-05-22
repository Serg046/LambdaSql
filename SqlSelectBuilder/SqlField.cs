using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Text;
using GuardExtensions;

namespace SqlSelectBuilder
{
    public enum FieldAggregation
    {
        // ReSharper disable InconsistentNaming
        MIN,
        MAX,
        AVG,
        SUM,
        COUNT
        // ReSharper restore InconsistentNaming
    }

    public class SqlField<TEntity> : ISqlField
    {
        internal SqlField()
        {
        }

        public ISqlAlias Alias { get; set; }
        public string Name { get; set; }
        public string AsAlias { get; set; }
        public FieldAggregation? Aggregation { get; set; }

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
                sb.Insert(0, Aggregation + "(").Append(")");
            return sb;
        }

        public override string ToString()
        {
            return View;
        }

        public static SqlField<TEntity> From<TType>(Expression<Func<TEntity, TType>> field, string asAlias = null)
        {
            Contract.Ensures(Contract.Result<SqlField<TEntity>>() != null);
            Guard.IsNotNull(field);
            return From(MetadataProvider.Instance.AliasFor<TEntity>(), field, asAlias);
        }

        public static SqlField<TEntity> From<TType>(SqlAlias<TEntity> alias, Expression<Func<TEntity, TType>> field, string asAlias = null)
        {
            Guard.IsNotNull(alias);
            Guard.IsNotNull(field);
            return AggregateResult(alias, field, null, asAlias);
        }

        public static SqlField<TEntity> Min<TType>(Expression<Func<TEntity, TType>> field, string asAlias = null)
        {
            Contract.Ensures(Contract.Result<SqlField<TEntity>>() != null);
            Guard.IsNotNull(field);
            return Min(MetadataProvider.Instance.AliasFor<TEntity>(), field, asAlias);
        }

        public static SqlField<TEntity> Min<TType>(SqlAlias<TEntity> alias, Expression<Func<TEntity, TType>> field, string asAlias = null)
        {
            Contract.Ensures(Contract.Result<SqlField<TEntity>>() != null);
            Guard.IsNotNull(alias);
            Guard.IsNotNull(field);
            return AggregateResult(alias, field, FieldAggregation.MIN, asAlias);
        }

        public static SqlField<TEntity> Max<TType>(Expression<Func<TEntity, TType>> field, string asAlias = null)
        {
            Contract.Ensures(Contract.Result<SqlField<TEntity>>() != null);
            Guard.IsNotNull(field);
            return Max(MetadataProvider.Instance.AliasFor<TEntity>(), field, asAlias);
        }

        public static SqlField<TEntity> Max<TType>(SqlAlias<TEntity> alias, Expression<Func<TEntity, TType>> field, string asAlias = null)
        {
            Contract.Ensures(Contract.Result<SqlField<TEntity>>() != null);
            Guard.IsNotNull(alias);
            Guard.IsNotNull(field);
            return AggregateResult(alias, field, FieldAggregation.MAX, asAlias);
        }

        public static SqlField<TEntity> Avg<TType>(Expression<Func<TEntity, TType>> field, string asAlias = null)
        {
            Contract.Ensures(Contract.Result<SqlField<TEntity>>() != null);
            Guard.IsNotNull(field);
            return Avg(MetadataProvider.Instance.AliasFor<TEntity>(), field, asAlias);
        }

        public static SqlField<TEntity> Avg<TType>(SqlAlias<TEntity> alias, Expression<Func<TEntity, TType>> field, string asAlias = null)
        {
            Contract.Ensures(Contract.Result<SqlField<TEntity>>() != null);
            Guard.IsNotNull(alias);
            Guard.IsNotNull(field);
            return AggregateResult(alias, field, FieldAggregation.AVG, asAlias);
        }

        public static SqlField<TEntity> Sum<TType>(Expression<Func<TEntity, TType>> field, string asAlias = null)
        {
            Contract.Ensures(Contract.Result<SqlField<TEntity>>() != null);
            Guard.IsNotNull(field);
            return Sum(MetadataProvider.Instance.AliasFor<TEntity>(), field, asAlias);
        }

        public static SqlField<TEntity> Sum<TType>(SqlAlias<TEntity> alias, Expression<Func<TEntity, TType>> field, string asAlias = null)
        {
            Contract.Ensures(Contract.Result<SqlField<TEntity>>() != null);
            Guard.IsNotNull(alias);
            Guard.IsNotNull(field);
            return AggregateResult(alias, field, FieldAggregation.SUM, asAlias);
        }

        public static SqlField<TEntity> Count<TType>(Expression<Func<TEntity, TType>> field, string asAlias = null)
        {
            Contract.Ensures(Contract.Result<SqlField<TEntity>>() != null);
            Guard.IsNotNull(field);
            return Count(MetadataProvider.Instance.AliasFor<TEntity>(), field, asAlias);
        }

        public static SqlField<TEntity> Count<TType>(SqlAlias<TEntity> alias, Expression<Func<TEntity, TType>> field, string asAlias = null)
        {
            Contract.Ensures(Contract.Result<SqlField<TEntity>>() != null);
            Guard.IsNotNull(alias);
            Guard.IsNotNull(field);
            return AggregateResult(alias, field, FieldAggregation.COUNT, asAlias);
        }

        private static SqlField<TEntity> AggregateResult<TType>(SqlAlias<TEntity> alias,
            Expression<Func<TEntity, TType>> field, FieldAggregation? aggregation, string asAlias)
        {
            Contract.Ensures(Contract.Result<SqlField<TEntity>>() != null);
            Guard.IsNotNull(alias);
            Guard.IsNotNull(field);

            var fieldName = MetadataProvider.Instance.GetPropertyName(field);
            return new SqlField<TEntity>() { Alias = alias, Name = fieldName, AsAlias = asAlias, Aggregation = aggregation };
        }
    }
}

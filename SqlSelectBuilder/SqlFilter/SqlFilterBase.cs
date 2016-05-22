using System.Linq;
using System.Linq.Expressions;
using GuardExtensions;

namespace SqlSelectBuilder.SqlFilter
{
    public interface ISqlFilter
    {
        string Filter { get; }
    }

    public interface ISqlFilterItems
    {
        ImmutableList<ISqlFilterItem> FilterItems { get; }
    }

    public class SqlFilterBase<TEntity> : ISqlFilter, ISqlFilterItems
    {
        protected const string AND = " AND ";
        protected const string OR = " OR ";
        protected bool MustBeWithoutAliases = false;
        
        internal SqlFilterBase(ImmutableList<ISqlFilterItem> sqlFilterItems)
        {
            Guard.IsNotNull(sqlFilterItems);
            FilterItems = sqlFilterItems;
        }

        internal ImmutableList<ISqlFilterItem> FilterItems { get; }
        ImmutableList<ISqlFilterItem> ISqlFilterItems.FilterItems => FilterItems;

        public string Filter => FilterItems.Aggregate(string.Empty,
            (s, item) => s + item.ToString(MustBeWithoutAliases));

        public override string ToString() => Filter;

        protected static SqlAlias<TEntity> CheckAlias(SqlAlias<TEntity> alias)
        {
            return alias ?? MetadataProvider.Instance.AliasFor<TEntity>();
        }

        protected static string GetFieldName(LambdaExpression field, ISqlAlias alias)
        {
            var fieldName = MetadataProvider.Instance.GetPropertyName(field);
            return alias.Value + "." + fieldName;
        }

        internal static SqlFilterItem BuildSqlFilterItem(LambdaExpression field, SqlAlias<TEntity> alias)
        {
            Guard.IsNotNull(field);
            alias = CheckAlias(alias);
            var sqlField = new SqlField<TEntity>() { Alias = alias, Name = MetadataProvider.Instance.GetPropertyName(field)};
            return new SqlFilterItem(sqlField);
        }

        internal static SqlFilterItem BuildSqlFilterItem(ISqlField sqlField)
        {
            Guard.IsNotNull(sqlField);
            return new SqlFilterItem(sqlField);
        }
    }
}

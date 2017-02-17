using System.Collections.Immutable;
using System.Linq.Expressions;
using GuardExtensions;
using LambdaSqlBuilder.SqlFilter.SqlFilterItem;

namespace LambdaSqlBuilder.SqlFilter
{
    internal delegate ISqlFilterItem SqlFilterItemFunc(SqlFilterConfiguration configuration);

    public class SqlFilterBase : ISqlFilter
    {
        protected bool MustBeWithoutAliases = false;
        
        internal SqlFilterBase(ImmutableList<SqlFilterItemFunc> sqlFilterItems)
        {
            Guard.IsNotNull(sqlFilterItems);
            FilterItems = sqlFilterItems;
        }

        internal ImmutableList<SqlFilterItemFunc> FilterItems { get; }

        public string Filter
        {
            get
            {
                var configuration = new SqlFilterConfiguration
                {
                    WithoutAliases = MustBeWithoutAliases,
                    WithoutParameters = true
                };
                var result = string.Empty;
                foreach (var item in FilterItems)
                    result = result + item(configuration).Expression;
                return result;
            }
        }

        public override string ToString() => Filter;

        protected static SqlAlias<TEntity> CheckAlias<TEntity>(SqlAlias<TEntity> alias)
            => alias ?? MetadataProvider.Instance.AliasFor<TEntity>();

        protected static string GetFieldName(LambdaExpression field, ISqlAlias alias)
            => alias.Value + "." + MetadataProvider.Instance.GetPropertyName(field);

        internal static ISqlField BuildSqlField<TEntity>(LambdaExpression field, SqlAlias<TEntity> alias)
        {
            alias = CheckAlias(alias);
            var sqlField = new SqlField<TEntity>() { Alias = alias, Name = MetadataProvider.Instance.GetPropertyName(field)};
            return sqlField;
        }
    }
}

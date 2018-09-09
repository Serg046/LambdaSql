using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using LambdaSql.Field;
using LambdaSql.Filter.SqlFilterItem;

namespace LambdaSql.Filter
{
    internal delegate ISqlFilterItem SqlFilterItemCallback(SqlFilterConfiguration configuration);

    public class SqlFilterBase : ISqlFilter
    {
        protected bool MustBeWithoutAliases = false;
        protected string ParamPrefix = "p";

        internal SqlFilterBase(ImmutableList<SqlFilterItemCallback> sqlFilterItems)
        {
            FilterItems = sqlFilterItems;
        }


        internal ImmutableList<SqlFilterItemCallback> FilterItems { get; }

        private string _rawSql;
        public string RawSql
        {
            get
            {
                if (_rawSql == null)
                {
                    var configuration = new SqlFilterConfiguration
                    {
                        WithoutAliases = MustBeWithoutAliases,
                        WithoutParameters = true
                    };

                    _rawSql = FilterItems.Aggregate(new StringBuilder(),
                        (sb, item) => sb.Append(item(configuration).Expression)).ToString();
                }
                return _rawSql;
            }
        }

        private DbParameter[] _parameters;
        public DbParameter[] Parameters
        {
            get
            {
                if (_parameters == null)
                    FillParametricFilter();
                return _parameters;
            }
        }

        private string _parametricSql;
        public string ParametricSql
        {
            get
            {
                if (_parametricSql == null)
                    FillParametricFilter();
                return _parametricSql;
            }
        }

        private void FillParametricFilter()
        {
            var configuration = new SqlFilterConfiguration
            {
                WithoutAliases = MustBeWithoutAliases,
                WithoutParameters = false
            };

            var filterSb = new StringBuilder();
            var parameters = new List<DbParameter>();
            var counter = 0;
            foreach (var itemFunc in FilterItems)
            {
                var item = itemFunc(configuration);
                parameters.AddRange(item.Parameters.Select(p =>
                {
                    p.ParameterName = $"@{ParamPrefix}{counter}";
                    counter++;
                    return p;
                }));
                filterSb.Append(item.Expression);
            }

            _parametricSql = filterSb.ToString();
            _parameters = parameters.ToArray();
        }

        public override string ToString() => RawSql;

        protected static SqlAlias<TEntity> CheckAlias<TEntity>(SqlAlias<TEntity> alias)
            => alias ?? MetadataProvider.Instance.AliasFor<TEntity>();

        protected static string GetFieldName(LambdaExpression field, ISqlAlias alias)
            => alias.Value + "." + MetadataProvider.Instance.GetPropertyName(field);

        internal static ITypedSqlField BuildSqlField<TEntity, TFieldType>(LambdaExpression field, SqlAlias<TEntity> alias)
        {
            alias = CheckAlias(alias);
            var sqlField = new SqlField<TEntity, TFieldType> { Alias = alias, Name = MetadataProvider.Instance.GetPropertyName(field)};
            return sqlField;
        }

        protected static void CheckField<TEntity, TFieldType>(ITypedSqlField field)
        {
            Debug.Assert(field != null, "SqlField is null.");
            Debug.Assert(typeof(TEntity) == field.EntityType,
                $"Incorrect SqlField, entity type does not match. Expected: {typeof(TEntity)}; Actual: {field.EntityType}.");
            Debug.Assert(typeof(TFieldType).IsAssignableFrom(field.FieldType),
                $"Incorrect SqlField, field type does not match. Expected: {typeof(TFieldType)}; Actual: {field.FieldType}.");
        }

        internal ImmutableList<SqlFilterItemCallback> AddItem(SqlFilterItemCallback item)
            => FilterItems.Count == 0 ? FilterItems : FilterItems.Add(item);

        ISqlFilter ISqlFilter.WithParameterPrefix(string prefix)
        {
            var filter = (SqlFilterBase)MemberwiseClone();
            filter.ParamPrefix = prefix;
            return filter;
        }
    }
}

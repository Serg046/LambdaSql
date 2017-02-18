using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using GuardExtensions;
using LambdaSqlBuilder.SqlFilter.SqlFilterItem;

namespace LambdaSqlBuilder.SqlFilter
{
    internal delegate ISqlFilterItem SqlFilterItemFunc(SqlFilterConfiguration configuration);

    public class SqlFilterBase : ISqlFilter
    {
        protected bool MustBeWithoutAliases = false;
        protected string ParamPrefix = "p";

        internal SqlFilterBase(ImmutableList<SqlFilterItemFunc> sqlFilterItems)
        {
            Guard.IsNotNull(sqlFilterItems);
            FilterItems = sqlFilterItems;
        }


        internal ImmutableList<SqlFilterItemFunc> FilterItems { get; }

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

        private SqlParameter[] _parameters;
        public SqlParameter[] Parameters
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
            var parameters = new List<SqlParameter>();
            var counter = 0;
            foreach (var itemFunc in FilterItems)
            {
                var item = itemFunc(configuration);
                filterSb.Append(item.Expression);
                parameters.AddRange(item.Parameters.Select(p =>
                {
                    p.ParameterName = ParamPrefix + counter;
                    counter++;
                    return p;
                }));
            }

            _parametricSql = filterSb.ToString();
            _parameters = parameters.ToArray();
        }

        public override string ToString() => RawSql;

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

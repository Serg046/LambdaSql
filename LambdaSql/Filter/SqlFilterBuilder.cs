using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using LambdaSql.Field;
using LambdaSql.Filter.SqlFilterItem;

namespace LambdaSql.Filter
{
    internal class SqlFilterBuilder<TEntity>
    {
        private readonly ImmutableList<SqlFilterItemCallback> _sqlFilterItems;
        private readonly ISqlField _sqlField;
        private readonly Func<ImmutableList<SqlFilterItemCallback>, SqlFilterBase> _filterCreatorFunc;

        public SqlFilterBuilder(ImmutableList<SqlFilterItemCallback> sqlFilterItems, ISqlField sqlField, Func<ImmutableList<SqlFilterItemCallback>, SqlFilterBase> filterCreatorFunc)
        {
            _sqlFilterItems = sqlFilterItems;
            _sqlField = sqlField;
            _filterCreatorFunc = filterCreatorFunc;
        }

        private ISqlAlias CheckAlias<T>(ISqlAlias alias)
            => alias ?? MetadataProvider.Instance.AliasFor<T>();

        private T BuildFilter<T>(ImmutableList<SqlFilterItemCallback> sqlFilterItems)
            => (dynamic)_filterCreatorFunc(sqlFilterItems);

        public T BuildFilter<T>(string expression, ISqlField sqlField)
            => BuildFilter<T>(_sqlFilterItems.Add(config
                => new SqlFilterItem.SqlFilterItem(expression, SqlFilterParameter.Create(config, sqlField))));

        public T BuildFilter<T>(string expression, params SqlFilterParameter[] args)
            => BuildFilter<T>(_sqlFilterItems.Add(config => new SqlFilterItem.SqlFilterItem(expression, args)));

        public T BuildFilter<T>(string expression, Func<SqlFilterConfiguration, SqlFilterParameter[]> args)
            => BuildFilter<T>(_sqlFilterItems.Add(config => new SqlFilterItem.SqlFilterItem(expression, args.Invoke(config))));

        public T ComparisonFilter<T>(string @operator, object value)
        {
            Func<SqlFilterConfiguration, SqlFilterParameter[]> args = config => new[]
            {
                SqlFilterParameter.Create(config, _sqlField),
                SqlFilterParameter.Create(config, value)
            };
            return BuildFilter<T>("{0} " + @operator + " {1}", args);
        }

        public T ComparisonFilter<T>(string @operator, ISqlField sqlField)
        {
            if (sqlField == null) throw new ArgumentNullException(nameof(sqlField));
            Func<SqlFilterConfiguration, SqlFilterParameter[]> args = config => new[]
            {
                SqlFilterParameter.Create(config, _sqlField),
                SqlFilterParameter.Create(config, sqlField)
            };
            return BuildFilter<T>("{0} " + @operator + " {1}", args);
        }

        public T ComparisonFilter<T>(string @operator, LambdaExpression field, SqlAlias<TEntity> alias)
            => ComparisonFilter<T, TEntity>(@operator, field, alias);

        public T ComparisonFilter<T, TAlias>(string logicOperator, LambdaExpression field, ISqlAlias alias)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            alias = CheckAlias<TAlias>(alias);
            var sqlField = new SqlField<TEntity>() { Alias = alias, Name = MetadataProvider.Instance.GetPropertyName(field) };
            return ComparisonFilter<T>(logicOperator, sqlField);
        }

        public T ContainsFilter<T, TType>(string @operator, IEnumerable<TType> values)
        {
            Func<SqlFilterConfiguration, SqlFilterParameter[]> args = config =>
            {
                var list = new List<SqlFilterParameter>();
                list.Add(SqlFilterParameter.Create(config, _sqlField));
                list.AddRange(values.Select(val => SqlFilterParameter.Create(config, val)));
                return list.ToArray();
            };

            var parameters = string.Join(",", values.Select((val, i) => $"{{{i + 1}}}"));
            return BuildFilter<T>("{0} " + @operator + " (" + parameters + ")", args);
        }
    }
}

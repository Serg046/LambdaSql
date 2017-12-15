using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using LambdaSql.Field;
using LambdaSql.Filter;

namespace LambdaSql.QueryBuilder
{
    public delegate string ModifyQueryPartCallback(string selectFields);

    internal abstract class SqlSelectQueryBuilderBase : ISqlSelectQueryBuilder
    {
        protected const string SEPARATOR = "\r\n";
        protected const string SEPARATOR_WITH_OFFSET = "\r\n    ";

        object ICloneable.Clone()
        {
            return MemberwiseClone();
        }

        private SqlSelectQueryBuilderBase Clone()
        {
            return (SqlSelectQueryBuilderBase)MemberwiseClone();
        }

        private ImmutableList<ModifyQueryPartCallback> _selectFieldsModificators = ImmutableList<ModifyQueryPartCallback>.Empty;
        public ISqlSelectQueryBuilder ModifySelectFields(ModifyQueryPartCallback modificationCallback)
        {
            var clone = Clone();
            clone._selectFieldsModificators = clone._selectFieldsModificators.Add(modificationCallback);
            return clone;
        }

        private ImmutableList<ModifyQueryPartCallback> _groupByFieldsModificators = ImmutableList<ModifyQueryPartCallback>.Empty;
        public ISqlSelectQueryBuilder ModifyGroupByFields(ModifyQueryPartCallback modificationCallback)
        {
            var clone = Clone();
            clone._groupByFieldsModificators = clone._groupByFieldsModificators.Add(modificationCallback);
            return clone;
        }

        private ImmutableList<ModifyQueryPartCallback> _orderByFieldsModificators = ImmutableList<ModifyQueryPartCallback>.Empty;
        public ISqlSelectQueryBuilder ModifyOrderByFields(ModifyQueryPartCallback modificationCallback)
        {
            var clone = Clone();
            clone._orderByFieldsModificators = clone._orderByFieldsModificators.Add(modificationCallback);
            return clone;
        }

        private ImmutableList<ModifyQueryPartCallback> _joinModificators = ImmutableList<ModifyQueryPartCallback>.Empty;
        public ISqlSelectQueryBuilder ModifyJoins(ModifyQueryPartCallback modificationCallback)
        {
            var clone = Clone();
            clone._joinModificators = clone._joinModificators.Add(modificationCallback);
            return clone;
        }

        private ImmutableList<ModifyQueryPartCallback> _whereModificators = ImmutableList<ModifyQueryPartCallback>.Empty;
        public ISqlSelectQueryBuilder ModifyWhereFilters(ModifyQueryPartCallback modificationCallback)
        {
            var clone = Clone();
            clone._whereModificators = clone._whereModificators.Add(modificationCallback);
            return clone;
        }

        private ImmutableList<ModifyQueryPartCallback> _havingModificators = ImmutableList<ModifyQueryPartCallback>.Empty;
        public ISqlSelectQueryBuilder ModifyHavingFilters(ModifyQueryPartCallback modificationCallback)
        {
            var clone = Clone();
            clone._havingModificators = clone._havingModificators.Add(modificationCallback);
            return clone;
        }

        public abstract string Build(SqlSelectInfo info, bool parametric);

        protected void CheckAsAliases(SqlSelectInfo info)
        {
            // ReSharper disable PossibleMultipleEnumeration
            var allFields = info.SelectFields().Union(info.GroupByFields()).Union(info.OrderByFields());
            var intersect = info.AllAliases.Select(a => a.Value).Intersect(allFields.Select(a => a.AsAlias));
            if (intersect.Any())
                throw new IncorrectAliasException($"The following user aliases are incorrect: {string.Join(", ", intersect)}.");
            // ReSharper restore PossibleMultipleEnumeration
        }

        protected string GetSelectedFields(SqlSelectInfo info)
        {
            var selectFields = info.SelectFields();
            var fields = selectFields.Count == 0 ? "*" : string.Join(", ", selectFields);
            fields = info.Distinct() ? $"DISTINCT {fields}" : fields;
            return _selectFieldsModificators.Aggregate(fields, (result, callback) => callback(result));
        }

        protected void AppendJoins(StringBuilder sb, SqlSelectInfo info)
        {
            if (info.Joins().Count > 0)
            {
                var joins = _joinModificators.Aggregate(string.Join(SEPARATOR, info.Joins()),
                    (result, callback) => callback(result));
                sb.Append(SEPARATOR).Append(joins);
            }
        }

        private void AppendFilter(StringBuilder sb, string clause, ISqlFilter filter, bool parametric,
            IEnumerable<ModifyQueryPartCallback> modificators)
        {
            if (filter != null)
            {
                var sql = parametric ? filter.ParametricSql : filter.RawSql;
                sb.Append(SEPARATOR).Append(clause)
                    .Append(SEPARATOR_WITH_OFFSET)
                    .Append(modificators.Aggregate(sql, (result, callback) => callback(result)));
            }
        }

        protected void AppendWhere(StringBuilder sb, SqlSelectInfo info, bool parametric)
        {
            AppendFilter(sb, "WHERE", info.Where(), parametric, _whereModificators);
        }

        protected void AppendHaving(StringBuilder sb, SqlSelectInfo info, bool parametric)
        {
            AppendFilter(sb, "HAVING", info.Having(), parametric, _havingModificators);
        }

        private void AppendFields(StringBuilder sb, string clause, ICollection<ISqlField> fields,
            IEnumerable<ModifyQueryPartCallback> modificators)
        {
            if (fields.Count > 0)
            {
                var joinedFields = string.Join(", ", fields.Select(f => f.ShortView));
                sb.Append(SEPARATOR)
                    .Append(clause)
                    .Append(SEPARATOR_WITH_OFFSET)
                    .Append(modificators.Aggregate(joinedFields, (result, callback) => callback(result)));
            }
        }

        protected void AppendGroupByFields(StringBuilder sb, SqlSelectInfo info)
        {
            AppendFields(sb, "GROUP BY", info.GroupByFields(), _groupByFieldsModificators);
        }

        protected void AppendOrderByFields(StringBuilder sb, SqlSelectInfo info)
        {
            AppendFields(sb, "ORDER BY", info.OrderByFields(), _orderByFieldsModificators);
        }
    }
}

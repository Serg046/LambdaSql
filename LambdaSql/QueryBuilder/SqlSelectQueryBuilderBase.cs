using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using LambdaSql.Field;
using LambdaSql.Filter;

namespace LambdaSql.QueryBuilder
{
    public delegate string ModifyQueryPartCallback(string query);

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

        private ImmutableList<ModifyQueryPartCallback> _wholeQueryModificators = ImmutableList<ModifyQueryPartCallback>.Empty;
        public ISqlSelectQueryBuilder Modify(ModifyQueryPartCallback modificationCallback)
        {
            var clone = Clone();
            clone._wholeQueryModificators = clone._wholeQueryModificators.Add(modificationCallback);
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

        protected void AppendJoins(StringBuilder querySb, SqlSelectInfo info)
        {
            if (info.Joins().Count > 0)
            {
                var joins = _joinModificators.Aggregate(string.Join(SEPARATOR, info.Joins()),
                    (result, callback) => callback(result));
                querySb.Append(SEPARATOR).Append(joins);
            }
        }

        private void AppendFilter(StringBuilder querySb, string clause, ISqlFilter filter, bool parametric,
            IReadOnlyList<ModifyQueryPartCallback> modificators)
        {
            if (filter != null)
            {
                var sql = parametric ? filter.ParametricSql : filter.RawSql;
                querySb.Append(SEPARATOR).Append(clause)
                    .Append(SEPARATOR_WITH_OFFSET)
                    .Append(modificators.Aggregate(sql, (result, callback) => callback(result)));
            }
            else if (modificators.Count > 0)
            {
                querySb.Append(modificators.Aggregate(string.Empty, (result, callback) => callback(result)));
            }
        }

        protected void AppendWhere(StringBuilder querySb, SqlSelectInfo info, bool parametric)
        {
            AppendFilter(querySb, "WHERE", info.Where(), parametric, _whereModificators);
        }

        protected void AppendHaving(StringBuilder querySb, SqlSelectInfo info, bool parametric)
        {
            AppendFilter(querySb, "HAVING", info.Having(), parametric, _havingModificators);
        }

        private void AppendFields(StringBuilder querySb, string clause, ICollection<ISqlField> fields,
            IEnumerable<ModifyQueryPartCallback> modificators)
        {
            if (fields.Count > 0)
            {
                var joinedFields = string.Join(", ", fields.Select(f => f.ShortString));
                querySb.Append(SEPARATOR)
                    .Append(clause)
                    .Append(SEPARATOR_WITH_OFFSET)
                    .Append(modificators.Aggregate(joinedFields, (result, callback) => callback(result)));
            }
        }

        protected void AppendGroupByFields(StringBuilder querySb, SqlSelectInfo info)
        {
            AppendFields(querySb, "GROUP BY", info.GroupByFields(), _groupByFieldsModificators);
        }

        protected void AppendOrderByFields(StringBuilder querySb, SqlSelectInfo info)
        {
            AppendFields(querySb, "ORDER BY", info.OrderByFields(), _orderByFieldsModificators);
        }

        protected string GetQueryString(StringBuilder querySb)
            => _wholeQueryModificators.Aggregate(querySb.ToString(), (result, callback) => callback(result));
    }
}

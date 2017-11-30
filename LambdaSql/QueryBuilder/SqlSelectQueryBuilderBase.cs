using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using LambdaSql.Field;
using LambdaSql.Filter;

namespace LambdaSql.QueryBuilder
{
    public delegate string ModifySelectFieldsCallback(string selectFields);

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

        private ImmutableList<ModifySelectFieldsCallback> _selectFieldsModificators = ImmutableList<ModifySelectFieldsCallback>.Empty;
        public ISqlSelectQueryBuilder ModifySelectFields(ModifySelectFieldsCallback modificationCallback)
        {
            var clone = Clone();
            clone._selectFieldsModificators = clone._selectFieldsModificators.Add(modificationCallback);
            return clone;
        }

        public abstract string Build(SqlSelectInfo info);

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
            var joins = info.Joins();
            if (joins.Count > 0)
            {
                sb.Append(SEPARATOR).Append(string.Join(SEPARATOR, joins));
            }
        }

        protected void AppendFilter(StringBuilder sb, SqlSelectInfo info, string clause,
            Func<SqlSelectInfo, ISqlFilter> getFilterCallback)
        {
            var filter = getFilterCallback(info);
            if (filter != null)
            {
                sb.Append(SEPARATOR).Append(clause)
                    .Append(SEPARATOR_WITH_OFFSET).Append(filter.RawSql);
            }
        }

        protected void AppendFields(StringBuilder sb, SqlSelectInfo info, string clause,
            Func<SqlSelectInfo, ICollection<ISqlField>> getFieldsCallback)
        {
            var fields = getFieldsCallback(info);
            if (fields.Count > 0)
            {
                sb.Append(SEPARATOR).Append(clause)
                    .Append(SEPARATOR_WITH_OFFSET).Append(string.Join(", ", fields.Select(f => f.ShortView)));
            }
        }
    }
}

using LambdaSql.Field;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using LambdaSql.Filter;

namespace LambdaSql
{
    public class SqlSelectInfo : ICloneable
    {
        public SqlSelectInfo(ISqlAlias alias)
        {
            Alias = alias ?? throw new ArgumentNullException(nameof(alias));
        }

        public ISqlAlias Alias { get; }

        private IReadOnlyList<ISqlAlias> _allAliases;
        public IReadOnlyList<ISqlAlias> AllAliases => _allAliases ?? (_allAliases =
                                                          ImmutableList.CreateRange(new[] {Alias}.Concat(Joins()
                                                              .Select(j => j.JoinAlias))));

        object ICloneable.Clone()
        {
            return MemberwiseClone();
        }

        public SqlSelectInfo Clone()
        {
            return (SqlSelectInfo)MemberwiseClone();
        }

        private int? _top;
        public int? Top() => _top;
        public SqlSelectInfo Top(int? value)
        {
            var clone = Clone();
            clone._top = value;
            return clone;
        }

        private bool _distinct;
        public bool Distinct() => _distinct;
        public SqlSelectInfo Distinct(bool value)
        {
            var clone = Clone();
            clone._distinct = value;
            return clone;
        }

        private ImmutableList<ISqlField> _selectFields = ImmutableList<ISqlField>.Empty;
        public ImmutableList<ISqlField> SelectFields() => _selectFields;
        public SqlSelectInfo SelectFields(params ISqlField[] fields)
        {
            var clone = Clone();
            clone._selectFields = clone._selectFields.AddRange(fields);
            return clone;
        }

        private ImmutableList<ISqlField> _groupByFields = ImmutableList<ISqlField>.Empty;
        public ImmutableList<ISqlField> GroupByFields() => _groupByFields;
        public SqlSelectInfo GroupByFields(params ISqlField[] fields)
        {
            var clone = Clone();
            clone._groupByFields = clone._groupByFields.AddRange(fields);
            return clone;
        }

        private ImmutableList<ISqlField> _orderByFields = ImmutableList<ISqlField>.Empty;
        public ImmutableList<ISqlField> OrderByFields() => _orderByFields;
        public SqlSelectInfo OrderByFields(params ISqlField[] fields)
        {
            var clone = Clone();
            clone._orderByFields = clone._orderByFields.AddRange(fields);
            return clone;
        }

        private ImmutableList<ISqlJoin> _joins = ImmutableList<ISqlJoin>.Empty;
        public ImmutableList<ISqlJoin> Joins() => _joins;
        public SqlSelectInfo Joins(params ISqlJoin[] joins)
        {
            var clone = Clone();
            clone._joins = clone._joins.AddRange(joins);
            return clone;
        }

        private ISqlFilter _where;
        public ISqlFilter Where() => _where;
        public SqlSelectInfo Where(ISqlFilter value)
        {
            var clone = Clone();
            clone._where = value;
            return clone;
        }

        private ISqlFilter _having;
        public ISqlFilter Having() => _having;
        public SqlSelectInfo Having(ISqlFilter value)
        {
            var clone = Clone();
            clone._having = value;
            return clone;
        }
    }
}

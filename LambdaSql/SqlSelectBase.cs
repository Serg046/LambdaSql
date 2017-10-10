using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using GuardExtensions;
using LambdaSql.Field;
using LambdaSql.Filter;

namespace LambdaSql
{
    public class SqlSelectBase : ISqlSelectInfo
    {
        protected const string SEPARATOR = "\r\n";
        protected const string SEPARATOR_WITH_OFFSET = "\r\n    ";

        protected readonly IMetadataProvider MetadataProvider = LambdaSql.MetadataProvider.Instance;
        private readonly ISqlAlias _alias;
        protected int? TopLimit;
        protected bool TopByPercent;
        protected bool IsDistinct;
        protected readonly List<ISqlField> SelectFields;
        protected readonly List<ISqlField> GroupByFields;
        protected readonly List<ISqlField> OrderByFields;
        protected readonly List<ISqlJoin> Joins;
        protected ISqlFilter WhereFilter;
        protected ISqlFilter HavingFilter;

        public SqlSelectBase(ISqlAlias alias)
        {
            _alias = alias;
            Info = new SqlInfo(this);
            SelectFields = new List<ISqlField>();
            GroupByFields = new List<ISqlField>();
            OrderByFields = new List<ISqlField>();
            Joins = new List<ISqlJoin>();
        }

        public SqlInfo Info { get; }

        IEnumerable<ISqlField> ISqlSelectInfo.SelectFields => Info.SelectFields;
        IEnumerable<ISqlField> ISqlSelectInfo.GroupByFields => Info.GroupByFields;
        IEnumerable<ISqlField> ISqlSelectInfo.OrderByFields => Info.OrderByFields;
        IEnumerable<ISqlAlias> ISqlSelectInfo.TableAliases => Info.TableAliases;
        ISqlFilter ISqlSelectInfo.WhereFilter => Info.Where;
        ISqlFilter ISqlSelectInfo.HavingFilter => Info.Having;

        protected SqlField<TEntity> CreateSqlField<TEntity>(string name, ISqlAlias alias)
        {
            Guard.IsNotEmpty(name);
            Guard.IsNotNull(alias);
            return new SqlField<TEntity>()
            {
                Name = name,
                Alias = alias
            };
        }

        protected void AddFields<TEntity>(ISqlAlias alias, IEnumerable<Expression<Func<TEntity, object>>> fields, List<ISqlField> list)
        {
            if (fields == null)
                throw new ArgumentNullException(nameof(fields));

            list.AddRange(fields.Select(f => CreateSqlField<TEntity>(MetadataProvider.GetPropertyName(f), alias)));
        }

        protected ISqlFilter GetJoinFilter<TLeft, TJoin>(BinaryExpression expression, SqlAlias<TLeft> leftAlias, SqlAlias<TJoin> joinAlias)
        {
            Guard.IsNotNull(leftAlias);
            Guard.IsNotNull(joinAlias);
            if (expression == null || expression.NodeType != ExpressionType.Equal)
                throw new JoinException("Invalid join expression");

            var leftExpr = LibHelper.GetMemberExpression(expression.Left);
            var rightExpr = LibHelper.GetMemberExpression(expression.Right);

            Guard.IsNotNull(leftExpr);
            Guard.IsNotNull(rightExpr);

            var rightField = leftExpr.Expression.Type == leftAlias.EntityType
                ? CreateSqlField<TJoin>(MetadataProvider.GetPropertyName(rightExpr), joinAlias)
                : (ISqlField)CreateSqlField<TLeft>(MetadataProvider.GetPropertyName(rightExpr), leftAlias);

            return leftExpr.Expression.Type == leftAlias.EntityType
                ? SqlFilter<TLeft>.From<object>(SqlField.From(typeof(TLeft), typeof(int),
                    leftAlias, MetadataProvider.GetPropertyName(leftExpr))).EqualTo(rightField)
                : (ISqlFilter)SqlFilter<TJoin>.From<object>(SqlField.From(typeof(TJoin), typeof(int),
                    joinAlias, MetadataProvider.GetPropertyName(leftExpr))).EqualTo(rightField);
        }

        protected void Join<TJoin>(JoinType joinType, ISqlFilter condition, SqlAlias<TJoin> joinAlias = null)
        {
            Guard.IsNotNull(condition);
            if (joinAlias == null)
                joinAlias = MetadataProvider.AliasFor<TJoin>();
            if (Joins.Any(j => j.JoinAlias.Value == joinAlias.Value))
                throw new JoinException($"Alias '{joinAlias.Value}' is already registered");

            var join = new SqlJoin<TJoin>(joinType, condition, joinAlias);
            Joins.Add(join);
        }

        protected void CheckAsAliases()
        {
            // ReSharper disable PossibleMultipleEnumeration
            var allFields = SelectFields.Union(GroupByFields).Union(OrderByFields);
            var intersect = Info.TableAliases.Select(a => a.Value).Intersect(allFields.Select(a => a.AsAlias));
            if (intersect.Any())
                throw new IncorrectAliasException($"The following user aliases are incorrect: {string.Join(", ", intersect)}.");
            // ReSharper restore PossibleMultipleEnumeration
        }

        protected void SelectedFields(StringBuilder sb)
        {
            if (IsDistinct)
                sb.Append("DISTINCT ");
            if (TopLimit.HasValue)
            {
                sb.Append($"TOP {TopLimit} ");
                if (TopByPercent)
                    sb.Append("PERCENT ");
            }
            sb.Append(SelectFields.Count == 0 ? "*" : string.Join(", ", SelectFields)).Append(SEPARATOR);
        }

        public class SqlInfo
        {
            private readonly SqlSelectBase _select;

            public SqlInfo(SqlSelectBase select)
            {
                Guard.IsNotNull(select);
                _select = select;
            }

            public bool Distinct => _select.IsDistinct;
            public int? Top => _select.TopLimit;
            public IList<ISqlField> SelectFields => _select.SelectFields;
            public IList<ISqlField> GroupByFields => _select.GroupByFields;
            public IList<ISqlField> OrderByFields => _select.OrderByFields;
            public IList<ISqlJoin> Joins => _select.Joins;
            public ISqlFilter Where => _select.WhereFilter;
            public ISqlFilter Having => _select.HavingFilter;

            public IEnumerable<ISqlAlias> TableAliases
            {
                get
                {
                    yield return _select._alias;
                    foreach (var alias in Joins.Select(j => j.JoinAlias))
                        yield return alias;
                }
            } 
        }
    }
}

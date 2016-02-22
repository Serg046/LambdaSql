using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSelectBuilder
{
    public class SqlSelectBase<T>
    {
        protected readonly IMetadataProvider MetadataProvider = SqlSelectBuilder.MetadataProvider.Instance;
        protected int? TopLimit;
        protected bool TopByPercent;
        protected bool IsDistinct;
        protected readonly List<ISqlField> SelectFields;
        protected readonly List<ISqlField> GroupByFields;
        protected readonly List<ISqlField> OrderByFields;
        protected readonly List<ISqlJoin> Joins;
        protected ISqlFilter WhereFilter;
        protected ISqlFilter HavingFilter;

        public SqlSelectBase()
        {
            Info = new SqlInfo(this);
            SelectFields = new List<ISqlField>();
            GroupByFields = new List<ISqlField>();
            OrderByFields = new List<ISqlField>();
            Joins = new List<ISqlJoin>();
        }

        public SqlInfo Info { get; }

        protected SqlField<TEntity> CreateSqlField<TEntity>(string name, SqlAlias<TEntity> alias)
        {
            Contract.Requires(name.IsNotEmpty());
            Contract.Requires(alias != null);
            Contract.Ensures(Contract.Result<SqlField<TEntity>>() != null);
            return new SqlField<TEntity>()
            {
                Name = name,
                Alias = alias
            };
        }

        protected void AddFields<TEntity>(SqlAlias<TEntity> alias, IEnumerable<Expression<Func<TEntity, object>>> fields, List<ISqlField> list)
        {
            Contract.Requires(alias != null);
            if (fields == null)
                throw new ArgumentNullException(nameof(fields));

            list.AddRange(fields.Select(f => CreateSqlField(MetadataProvider.GetPropertyName(f), alias)));
        }

        protected SqlFilter<TJoin> GetJoinFilter<TLeft, TJoin>(BinaryExpression expression, SqlAlias<TLeft> leftAlias, SqlAlias<TJoin> joinAlias)
        {
            Contract.Requires(leftAlias != null);
            Contract.Requires(joinAlias != null);
            if (expression == null || expression.NodeType != ExpressionType.Equal)
                throw new JoinException("Invalid join expression");

            var leftField = CreateSqlField(MetadataProvider.GetPropertyName(expression.Left as MemberExpression), leftAlias);
            var rightField = CreateSqlField(MetadataProvider.GetPropertyName(expression.Right as MemberExpression), joinAlias);

            return SqlFilter<TLeft>.From<int>(leftField)
                .EqualTo(rightField)
                .Cast<TJoin>();
        }

        protected void Join<TJoin>(JoinType joinType, ISqlFilter condition, SqlAlias<TJoin> joinAlias = null)
        {
            if (joinAlias == null)
                joinAlias = MetadataProvider.AliasFor<TJoin>();
            if (Joins.Any(j => j.JoinAlias.Value == joinAlias.Value))
                throw new JoinException($"Alias '{joinAlias.Value}' is already registered");

            var join = new SqlJoin<TJoin>(joinType, condition, joinAlias);
            Joins.Add(join);
        }

        public class SqlInfo
        {
            private readonly SqlSelectBase<T> _select;

            public SqlInfo(SqlSelectBase<T> select)
            {
                _select = select;
            }

            public bool Distinct => _select.IsDistinct;
            public int? Top => _select.TopLimit;
            public IList<ISqlField> SelectFields => _select.SelectFields;
            public IList<ISqlField> GroupByFields => _select.GroupByFields;
            public IList<ISqlField> OrderByFields => _select.OrderByFields;
            public IList<ISqlJoin> Joins => _select.Joins;
            public ISqlFilter SqlFilter => _select.WhereFilter;

            public IEnumerable<ISqlAlias> Aliases
            {
                get
                {
                    yield return _select.MetadataProvider.AliasFor<T>();
                    foreach (var alias in Joins.Select(j => j.JoinAlias))
                        yield return alias;
                }
            } 
        }
    }
}

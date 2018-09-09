using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using LambdaSql.Field;
using LambdaSql.Filter;
using LambdaSql.QueryBuilder;

namespace LambdaSql
{
    public abstract class SqlSelectBase
    {
        protected const string SEPARATOR = "\r\n";
        protected const string SEPARATOR_WITH_OFFSET = "\r\n    ";

        protected readonly IMetadataProvider MetadataProvider = LambdaSql.MetadataProvider.Instance;

        protected SqlSelectBase(SqlSelectInfo info, ISqlSelectQueryBuilder queryBuilder)
        {
            Info = info;
            QueryBuilder = queryBuilder;
        }

        public SqlSelectInfo Info { get; }
        protected ISqlSelectQueryBuilder QueryBuilder { get; }

        private string _rawSql;
        public string RawSql => _rawSql ?? (_rawSql = QueryBuilder.Build(Info, parametric: false));

        private string _parametricSql;
        public string ParametricSql => _parametricSql ?? (_parametricSql = QueryBuilder.Build(Info, parametric: true));

        private ISqlField CreateSqlField<TEntity>(string name, ISqlAlias alias)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException(nameof(name));
            if (alias == null) throw new ArgumentNullException(nameof(alias));
            return new SqlField<TEntity>
            {
                Name = name,
                Alias = alias
            };
        }

        protected ISqlField[] CreateSqlFields<TEntity>(ISqlAlias alias, IEnumerable<Expression<Func<TEntity, object>>> fields)
        {
            return fields.Select(f => CreateSqlField<TEntity>(MetadataProvider.GetPropertyName(f), alias)).ToArray();
        }

        protected ISqlFilter GetJoinFilter<TLeft, TJoin>(BinaryExpression expression, SqlAlias<TLeft> leftAlias, SqlAlias<TJoin> joinAlias)
        {
            if (leftAlias == null) throw new ArgumentNullException(nameof(leftAlias));
            if (joinAlias == null) throw new ArgumentNullException(nameof(joinAlias));
            if (expression == null || expression.NodeType != ExpressionType.Equal)
                throw new JoinException("Invalid join expression");

            var leftExpr = LibHelper.GetMemberExpression(expression.Left);
            var rightExpr = LibHelper.GetMemberExpression(expression.Right);

            if (leftExpr == null) throw new ArgumentNullException(nameof(leftExpr));
            if (rightExpr == null) throw new ArgumentNullException(nameof(rightExpr));

            var rightField = leftExpr.Expression.Type == leftAlias.EntityType
                ? CreateSqlField<TJoin>(MetadataProvider.GetPropertyName(rightExpr), joinAlias)
                : (ISqlField)CreateSqlField<TLeft>(MetadataProvider.GetPropertyName(rightExpr), leftAlias);

            return leftExpr.Expression.Type == leftAlias.EntityType
                ? SqlFilter<TLeft>.From<object>(SqlField.From(typeof(TLeft), typeof(int),
                    leftAlias, MetadataProvider.GetPropertyName(leftExpr))).EqualTo(rightField)
                : (ISqlFilter)SqlFilter<TJoin>.From<object>(SqlField.From(typeof(TJoin), typeof(int),
                    joinAlias, MetadataProvider.GetPropertyName(leftExpr))).EqualTo(rightField);
        }

        protected SqlSelectInfo Join<TJoin>(JoinType joinType, ISqlFilter condition, SqlAlias<TJoin> joinAlias = null)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (joinAlias == null)
                joinAlias = MetadataProvider.AliasFor<TJoin>();
            if (Info.Joins().Any(j => j.JoinAlias.Value == joinAlias.Value))
                throw new JoinException($"Alias '{joinAlias.Value}' is already registered");

            var join = new SqlJoin<TJoin>(joinType, condition, joinAlias);
            return Info.Joins(join);
        }

        protected DbParameter[] GetFilterParameters(SqlSelectInfo info)
            => Info.Where()?.Parameters?
                   .Concat(Info.Having()?.Parameters ?? new DbParameter[0]).ToArray()
               ?? Info.Having()?.Parameters
               ?? new DbParameter[0];
    }
}

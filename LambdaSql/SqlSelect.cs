using System;
using System.Data.Common;
using System.Linq.Expressions;
using LambdaSql.Field;
using LambdaSql.Filter;
using LambdaSql.QueryBuilder;

namespace LambdaSql
{
    public class SqlSelect<T> : SqlSelectBase, ISqlSelect
    {
        public SqlSelect() : this(new SqlSelectInfo(LambdaSql.MetadataProvider.Instance.AliasFor<T>()),
            new SqlSelectQueryBuilder(LambdaSql.MetadataProvider.Instance.GetTableName<T>()))
        {
        }

        private SqlSelect(SqlSelectInfo info, ISqlSelectQueryBuilder queryBuilder) : base(info, queryBuilder)
        {
        }

        private SqlSelect<T> CreateSqlSelect(SqlSelectInfo info) => new SqlSelect<T>(info, QueryBuilder);

        ISqlSelect ISqlSelect.Extend(Func<ISqlSelectQueryBuilder, ISqlSelectQueryBuilder> decorationCallback) => Extend(decorationCallback);
        public SqlSelect<T> Extend(Func<ISqlSelectQueryBuilder, ISqlSelectQueryBuilder> decorationCallback)
            => new SqlSelect<T>(Info, decorationCallback(QueryBuilder));

        private DbParameter[] _parameters;
        public DbParameter[] Parameters => _parameters ?? (_parameters = GetFilterParameters(Info));

        public override string ToString() => RawSql;

        public Type EntityType => typeof(T);

        public SqlSelect<T> Distinct()
        {
            return Distinct(true);
        }

        ISqlSelect ISqlSelect.Distinct(bool isDistinct) => Distinct(isDistinct);
        public SqlSelect<T> Distinct(bool isDistinct)
        {
            return CreateSqlSelect(Info.Distinct(isDistinct));
        }

        //-------------------------------------------------------------------------

        ISqlSelect ISqlSelect.AddFields(ISqlField[] fields) => AddFields(fields);
        public SqlSelect<T> AddFields(params ISqlField[] fields)
        {
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            return CreateSqlSelect(Info.SelectFields(fields));
        }

        public SqlSelect<T> AddFields(params Expression<Func<T, object>>[] fields)
        {
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            return AddFields(CreateSqlFields(MetadataProvider.AliasFor<T>(), fields));
        }

        public SqlSelect<T> AddFields<TEntity>(params Expression<Func<TEntity, object>>[] fields)
        {
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            return AddFields(CreateSqlFields(MetadataProvider.AliasFor<TEntity>(), fields));
        }

        ISqlSelect ISqlSelect.AddFields<TEntity>(SqlAlias<TEntity> alias,
            params Expression<Func<TEntity, object>>[] fields)
        {
            if (alias == null)
                MetadataProvider.AliasFor<TEntity>();
            return AddFields(alias, fields);
        }

        public SqlSelect<T> AddFields<TEntity>(SqlAlias<TEntity> alias,
            params Expression<Func<TEntity, object>>[] fields)
        {
            if (alias == null) throw new ArgumentNullException(nameof(alias));
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            return AddFields(CreateSqlFields(alias, fields));
        }

        //-------------------------------------------------------------------------

        ISqlSelect ISqlSelect.GroupBy(ISqlField[] fields) => GroupBy(fields);
        public SqlSelect<T> GroupBy(params ISqlField[] fields)
        {
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            return CreateSqlSelect(Info.GroupByFields(fields));
        }

        public SqlSelect<T> GroupBy(params Expression<Func<T, object>>[] fields)
        {
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            return GroupBy(CreateSqlFields(MetadataProvider.AliasFor<T>(), fields));
        }

        public SqlSelect<T> GroupBy<TEntity>(params Expression<Func<TEntity, object>>[] fields)
        {
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            return GroupBy(CreateSqlFields(MetadataProvider.AliasFor<TEntity>(), fields));
        }

        ISqlSelect ISqlSelect.GroupBy<TEntity>(SqlAlias<TEntity> alias,
            params Expression<Func<TEntity, object>>[] fields)
        {
            if (alias == null)
                MetadataProvider.AliasFor<TEntity>();
            return GroupBy(alias, fields);
        }

        public SqlSelect<T> GroupBy<TEntity>(SqlAlias<TEntity> alias,
            params Expression<Func<TEntity, object>>[] fields)
        {
            if (alias == null) throw new ArgumentNullException(nameof(alias));
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            return GroupBy(CreateSqlFields(alias, fields));
        }

        //-------------------------------------------------------------------------

        ISqlSelect ISqlSelect.OrderBy(ISqlField[] fields) => OrderBy(fields);
        public SqlSelect<T> OrderBy(params ISqlField[] fields)
        {
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            return CreateSqlSelect(Info.OrderByFields(fields));
        }

        public SqlSelect<T> OrderBy(params Expression<Func<T, object>>[] fields)
        {
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            return OrderBy(CreateSqlFields(MetadataProvider.AliasFor<T>(), fields));
        }

        public SqlSelect<T> OrderBy<TEntity>(params Expression<Func<TEntity, object>>[] fields)
        {
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            return OrderBy(CreateSqlFields(MetadataProvider.AliasFor<TEntity>(), fields));
        }

        ISqlSelect ISqlSelect.OrderBy<TEntity>(SqlAlias<TEntity> alias,
            params Expression<Func<TEntity, object>>[] fields)
        {
            if (alias == null)
                MetadataProvider.AliasFor<TEntity>();
            return OrderBy(alias, fields);
        }

        public SqlSelect<T> OrderBy<TEntity>(SqlAlias<TEntity> alias,
            params Expression<Func<TEntity, object>>[] fields)
        {
            if (alias == null) throw new ArgumentNullException(nameof(alias));
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            return OrderBy(CreateSqlFields(alias, fields));
        }

        //-------------------------------------------------------------------------

        public SqlSelect<T> InnerJoin<TLeft, TJoin>(Expression<Func<TLeft, TJoin, bool>> condition,
            SqlAlias<TLeft> leftAlias = null, SqlAlias<TJoin> joinAlias = null)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (leftAlias == null)
                leftAlias = MetadataProvider.AliasFor<TLeft>();
            if (joinAlias == null)
                joinAlias = MetadataProvider.AliasFor<TJoin>();

            return InnerJoin(GetJoinFilter(condition.Body as BinaryExpression, leftAlias, joinAlias), joinAlias);
        }

        public SqlSelect<T> InnerJoin<TJoin>(ISqlFilter condition, SqlAlias<TJoin> joinAlias = null)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            return CreateSqlSelect(Join(JoinType.Inner, condition, joinAlias));
        }

        public SqlSelect<T> LeftJoin<TLeft, TJoin>(Expression<Func<TLeft, TJoin, bool>> condition,
            SqlAlias<TLeft> leftAlias = null, SqlAlias<TJoin> joinAlias = null)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (leftAlias == null)
                leftAlias = MetadataProvider.AliasFor<TLeft>();
            if (leftAlias == null)
                joinAlias = MetadataProvider.AliasFor<TJoin>();

            return LeftJoin(GetJoinFilter(condition.Body as BinaryExpression, leftAlias, joinAlias), joinAlias);
        }

        public SqlSelect<T> LeftJoin<TJoin>(ISqlFilter condition, SqlAlias<TJoin> joinAlias = null)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            return CreateSqlSelect(Join(JoinType.Left, condition, joinAlias));
        }

        public SqlSelect<T> RightJoin<TLeft, TJoin>(Expression<Func<TLeft, TJoin, bool>> condition,
            SqlAlias<TLeft> leftAlias = null, SqlAlias<TJoin> joinAlias = null)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (leftAlias == null)
                leftAlias = MetadataProvider.AliasFor<TLeft>();
            if (joinAlias == null)
                joinAlias = MetadataProvider.AliasFor<TJoin>();

            return RightJoin(GetJoinFilter(condition.Body as BinaryExpression, leftAlias, joinAlias), joinAlias);
        }

        public SqlSelect<T> RightJoin<TJoin>(ISqlFilter condition, SqlAlias<TJoin> joinAlias = null)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            return CreateSqlSelect(Join(JoinType.Right, condition, joinAlias));
        }

        public SqlSelect<T> FullJoin<TLeft, TJoin>(Expression<Func<TLeft, TJoin, bool>> condition,
            SqlAlias<TLeft> leftAlias = null, SqlAlias<TJoin> joinAlias = null)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (leftAlias == null)
                leftAlias = MetadataProvider.AliasFor<TLeft>();
            if (joinAlias == null)
                joinAlias = MetadataProvider.AliasFor<TJoin>();

            return FullJoin(GetJoinFilter(condition.Body as BinaryExpression, leftAlias, joinAlias), joinAlias);
        }

        public SqlSelect<T> FullJoin<TJoin>(ISqlFilter condition, SqlAlias<TJoin> joinAlias = null)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            return CreateSqlSelect(Join(JoinType.Full, condition, joinAlias));
        }

        ISqlSelect ISqlSelect.Join<TJoin>(JoinType joinType, ISqlFilter condition, SqlAlias<TJoin> joinAlias)
        {
            ISqlSelect result;
            switch (joinType)
            {
                case JoinType.Inner:
                    result = InnerJoin(condition, joinAlias);
                    break;
                case JoinType.Left:
                    result = LeftJoin(condition, joinAlias);
                    break;
                case JoinType.Right:
                    result = RightJoin(condition, joinAlias);
                    break;
                case JoinType.Full:
                    result = FullJoin(condition, joinAlias);
                    break;
                default:
                    throw new NotSupportedException($"{joinType.ToString()} is not supported");
            }
            return result;
        }

        //-------------------------------------------------------------------------

        ISqlSelect ISqlSelect.Where(ISqlFilter filter) => Where(filter);
        public SqlSelect<T> Where(ISqlFilter filter)
        {
            return CreateSqlSelect(Info.Where(filter.WithParameterPrefix("w")));
        }

        ISqlSelect ISqlSelect.Having(ISqlFilter filter) => Having(filter);
        public SqlSelect<T> Having(ISqlFilter filter)
        {
            return CreateSqlSelect(Info.Having(filter.WithParameterPrefix("h")));
        }
    }
}

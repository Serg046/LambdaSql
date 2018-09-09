using System;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using LambdaSql.Field;
using LambdaSql.Filter;
using LambdaSql.QueryBuilder;

namespace LambdaSql
{
    public class SqlSelect : SqlSelectBase, ISqlSelect
    {
        private readonly ISqlSelect _innerSqlSelect;

        public SqlSelect(ISqlSelect innerSqlSelect, ISqlAlias alias) 
            : this(innerSqlSelect, new SqlSelectInfo(alias),
                  new SqlSelectWrapperQueryBuilder(innerSqlSelect))
        {
        }

        private SqlSelect(ISqlSelect innerSqlSelect, SqlSelectInfo info, ISqlSelectQueryBuilder queryBuilder) : base(info, queryBuilder)
        {
            _innerSqlSelect = innerSqlSelect ?? throw new ArgumentNullException(nameof(innerSqlSelect));
        }

        private SqlSelect CreateSqlSelect(SqlSelectInfo info) => new SqlSelect(_innerSqlSelect, info, QueryBuilder);
        
        ISqlSelect ISqlSelect.Extend(Func<ISqlSelectQueryBuilder, ISqlSelectQueryBuilder> decorationCallback) => Extend(decorationCallback);
        public SqlSelect Extend(Func<ISqlSelectQueryBuilder, ISqlSelectQueryBuilder> decorationCallback)
            => new SqlSelect(_innerSqlSelect, Info, decorationCallback(QueryBuilder));

        private DbParameter[] _parameters;
        public DbParameter[] Parameters => _parameters
            ?? (_parameters = _innerSqlSelect.Parameters.Concat(GetFilterParameters(Info)).ToArray());

        public override string ToString() => RawSql;

        public Type EntityType => null;

        public SqlSelect Distinct()
        {
            return Distinct(true);
        }

        ISqlSelect ISqlSelect.Distinct(bool isDistinct) => Distinct(isDistinct);
        public SqlSelect Distinct(bool isDistinct)
        {
            return CreateSqlSelect(Info.Distinct(isDistinct));
        }

        //-------------------------------------------------------------------------

        ISqlSelect ISqlSelect.AddFields(ISqlField[] fields) => AddFields(fields);
        public SqlSelect AddFields(params ISqlField[] fields)
        {
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            return CreateSqlSelect(Info.SelectFields(fields.Select(f =>
            {
                f.Alias = Info.Alias;
                return f;
            }).ToArray()));
        }

        public SqlSelect AddFields<TEntity>(params Expression<Func<TEntity, object>>[] fields)
        {
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            return AddFields(CreateSqlFields(Info.Alias, fields));
        }

        ISqlSelect ISqlSelect.AddFields<TEntity>(SqlAlias<TEntity> alias, params Expression<Func<TEntity, object>>[] fields)
            => AddFields(fields);

        //-------------------------------------------------------------------------

        ISqlSelect ISqlSelect.GroupBy(ISqlField[] fields) => GroupBy(fields);
        public SqlSelect GroupBy(params ISqlField[] fields)
        {
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            return CreateSqlSelect(Info.GroupByFields(fields.Select(f =>
            {
                f.Alias = Info.Alias;
                return f;
            }).ToArray()));
        }

        public SqlSelect GroupBy<TEntity>(params Expression<Func<TEntity, object>>[] fields)
        {
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            return GroupBy(CreateSqlFields(Info.Alias, fields));
        }

        ISqlSelect ISqlSelect.GroupBy<TEntity>(SqlAlias<TEntity> alias, params Expression<Func<TEntity, object>>[] fields)
            => GroupBy(fields);

        //-------------------------------------------------------------------------

        ISqlSelect ISqlSelect.OrderBy(ISqlField[] fields) => OrderBy(fields);
        public SqlSelect OrderBy(params ISqlField[] fields)
        {
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            return CreateSqlSelect(Info.OrderByFields(fields.Select(f =>
            {
                f.Alias = Info.Alias;
                return f;
            }).ToArray()));
        }

        public SqlSelect OrderBy<TEntity>(params Expression<Func<TEntity, object>>[] fields)
        {
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            return OrderBy(CreateSqlFields(Info.Alias, fields));
        }

        ISqlSelect ISqlSelect.OrderBy<TEntity>(SqlAlias<TEntity> alias, params Expression<Func<TEntity, object>>[] fields)
            => OrderBy(fields);

        //-------------------------------------------------------------------------

        public SqlSelect InnerJoin<TLeft, TJoin>(Expression<Func<TLeft, TJoin, bool>> condition,
            SqlAlias<TLeft> leftAlias = null, SqlAlias<TJoin> joinAlias = null)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (leftAlias == null)
                leftAlias = MetadataProvider.AliasFor<TLeft>();
            if (joinAlias == null)
                joinAlias = MetadataProvider.AliasFor<TJoin>();

            return InnerJoin(GetJoinFilter(condition.Body as BinaryExpression, leftAlias, joinAlias), joinAlias);
        }

        public SqlSelect InnerJoin<TJoin>(ISqlFilter condition, SqlAlias<TJoin> joinAlias = null)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            return CreateSqlSelect(Join(JoinType.Inner, condition, joinAlias));
        }

        public SqlSelect LeftJoin<TLeft, TJoin>(Expression<Func<TLeft, TJoin, bool>> condition,
            SqlAlias<TLeft> leftAlias = null, SqlAlias<TJoin> joinAlias = null)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (leftAlias == null)
                leftAlias = MetadataProvider.AliasFor<TLeft>();
            if (leftAlias == null)
                joinAlias = MetadataProvider.AliasFor<TJoin>();

            return LeftJoin(GetJoinFilter(condition.Body as BinaryExpression, leftAlias, joinAlias), joinAlias);
        }

        public SqlSelect LeftJoin<TJoin>(ISqlFilter condition, SqlAlias<TJoin> joinAlias = null)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            return CreateSqlSelect(Join(JoinType.Left, condition, joinAlias));
        }

        public SqlSelect RightJoin<TLeft, TJoin>(Expression<Func<TLeft, TJoin, bool>> condition,
            SqlAlias<TLeft> leftAlias = null, SqlAlias<TJoin> joinAlias = null)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (leftAlias == null)
                leftAlias = MetadataProvider.AliasFor<TLeft>();
            if (joinAlias == null)
                joinAlias = MetadataProvider.AliasFor<TJoin>();

            return RightJoin(GetJoinFilter(condition.Body as BinaryExpression, leftAlias, joinAlias), joinAlias);
        }

        public SqlSelect RightJoin<TJoin>(ISqlFilter condition, SqlAlias<TJoin> joinAlias = null)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            return CreateSqlSelect(Join(JoinType.Right, condition, joinAlias));
        }

        public SqlSelect FullJoin<TLeft, TJoin>(Expression<Func<TLeft, TJoin, bool>> condition,
            SqlAlias<TLeft> leftAlias = null, SqlAlias<TJoin> joinAlias = null)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (leftAlias == null)
                leftAlias = MetadataProvider.AliasFor<TLeft>();
            if (joinAlias == null)
                joinAlias = MetadataProvider.AliasFor<TJoin>();

            return FullJoin(GetJoinFilter(condition.Body as BinaryExpression, leftAlias, joinAlias), joinAlias);
        }

        public SqlSelect FullJoin<TJoin>(ISqlFilter condition, SqlAlias<TJoin> joinAlias = null)
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
        public SqlSelect Where(ISqlFilter filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));
            return CreateSqlSelect(Info.Where(filter.WithParameterPrefix($"{Info.Alias.Value}_w")));
        }

        ISqlSelect ISqlSelect.Having(ISqlFilter filter) => Having(filter);
        public SqlSelect Having(ISqlFilter filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));
            return CreateSqlSelect(Info.Having(filter.WithParameterPrefix($"{Info.Alias.Value}_h")));
        }
    }
}

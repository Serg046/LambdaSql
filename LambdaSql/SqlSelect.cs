using System;
using System.Linq.Expressions;
using System.Text;
using GuardExtensions;
using LambdaSql.Field;
using LambdaSql.Filter;

namespace LambdaSql
{
    public class SqlSelect<T> : SqlSelectBase, ISqlSelect
    {
        public SqlSelect() : base(LambdaSql.MetadataProvider.Instance.AliasFor<T>())
        {
        }

        private SqlSelect(SqlSelectInfo info) : base(info)
        {
        }

        public string CommandText
        {
            get
            {
                CheckAsAliases();
                var sb = new StringBuilder("SELECT").Append(SEPARATOR_WITH_OFFSET);
                SelectedFields(sb);
                sb.Append("FROM").Append(SEPARATOR_WITH_OFFSET)
                    .Append(MetadataProvider.GetTableName<T>())
                    .Append(" ").Append(MetadataProvider.AliasFor<T>().Value);
                var joins = Info.Joins();
                if (joins.Count > 0)
                {
                    sb.Append(SEPARATOR).Append(string.Join(SEPARATOR, joins));
                }
                var where = Info.Where();
                if (where != null)
                {
                    sb.Append(SEPARATOR).Append("WHERE")
                        .Append(SEPARATOR_WITH_OFFSET).Append(where.RawSql);
                }
                var groupByFields = Info.GroupByFields();
                if (groupByFields.Count > 0)
                {
                    sb.Append(SEPARATOR).Append("GROUP BY")
                        .Append(SEPARATOR_WITH_OFFSET).Append(string.Join(", ", groupByFields));
                }
                var having = Info.Having();
                if (having != null)
                {
                    sb.Append(SEPARATOR).Append("HAVING")
                        .Append(SEPARATOR_WITH_OFFSET).Append(having.RawSql);
                }
                var orderByFields = Info.OrderByFields();
                if (orderByFields.Count > 0)
                {
                    sb.Append(SEPARATOR).Append("ORDER BY")
                        .Append(SEPARATOR_WITH_OFFSET).Append(string.Join(", ", orderByFields));
                }
                return sb.ToString();
            }
        }

        public override string ToString()
        {
            return CommandText;
        }

        public Type EntityType => typeof(T);

        ISqlSelect ISqlSelect.Top(int top) => Top(top);
        public SqlSelect<T> Top(int top)
        {
            if (top < 1)
            {
                throw new InvalidOperationException("The value must be greather than 0");
            }
            return new SqlSelect<T>(Info.Top(top));
        }

        public SqlSelect<T> Distinct()
        {
            return Distinct(true);
        }

        ISqlSelect ISqlSelect.Distinct(bool isDistinct) => Distinct(isDistinct);
        public SqlSelect<T> Distinct(bool isDistinct)
        {
            return new SqlSelect<T>(Info.Distinct(isDistinct));
        }

        //-------------------------------------------------------------------------

        ISqlSelect ISqlSelect.AddFields(ISqlField[] fields) => AddFields(fields);
        public SqlSelect<T> AddFields(params ISqlField[] fields)
        {
            Guard.IsNotNull(fields);
            return new SqlSelect<T>(Info.SelectFields(fields));
        }

        public SqlSelect<T> AddFields(params Expression<Func<T, object>>[] fields)
        {
            Guard.IsNotNull(fields);
            return AddFields(CreateSqlFields(MetadataProvider.AliasFor<T>(), fields));
        }

        public SqlSelect<T> AddFields<TEntity>(params Expression<Func<TEntity, object>>[] fields)
        {
            Guard.IsNotNull(fields);
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
            Guard.IsNotNull(alias);
            Guard.IsNotNull(fields);
            return AddFields(CreateSqlFields(alias, fields));
        }

        //-------------------------------------------------------------------------

        ISqlSelect ISqlSelect.GroupBy(ISqlField[] fields) => GroupBy(fields);
        public SqlSelect<T> GroupBy(params ISqlField[] fields)
        {
            Guard.IsNotNull(fields);
            return new SqlSelect<T>(Info.GroupByFields(fields));
        }

        public SqlSelect<T> GroupBy(params Expression<Func<T, object>>[] fields)
        {
            Guard.IsNotNull(fields);
            return GroupBy(CreateSqlFields(MetadataProvider.AliasFor<T>(), fields));
        }

        public SqlSelect<T> GroupBy<TEntity>(params Expression<Func<TEntity, object>>[] fields)
        {
            Guard.IsNotNull(fields);
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
            Guard.IsNotNull(alias);
            Guard.IsNotNull(fields);
            return GroupBy(CreateSqlFields(alias, fields));
        }

        //-------------------------------------------------------------------------

        ISqlSelect ISqlSelect.OrderBy(ISqlField[] fields) => OrderBy(fields);
        public SqlSelect<T> OrderBy(params ISqlField[] fields)
        {
            Guard.IsNotNull(fields);
            return new SqlSelect<T>(Info.OrderByFields(fields));
        }

        public SqlSelect<T> OrderBy(params Expression<Func<T, object>>[] fields)
        {
            Guard.IsNotNull(fields);
            return OrderBy(CreateSqlFields(MetadataProvider.AliasFor<T>(), fields));
        }

        public SqlSelect<T> OrderBy<TEntity>(params Expression<Func<TEntity, object>>[] fields)
        {
            Guard.IsNotNull(fields);
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
            Guard.IsNotNull(alias);
            Guard.IsNotNull(fields);
            return OrderBy(CreateSqlFields(alias, fields));
        }

        //-------------------------------------------------------------------------

        public SqlSelect<T> InnerJoin<TLeft, TJoin>(Expression<Func<TLeft, TJoin, bool>> condition,
            SqlAlias<TLeft> leftAlias = null, SqlAlias<TJoin> joinAlias = null)
        {
            Guard.IsNotNull(condition);
            if (leftAlias == null)
                leftAlias = MetadataProvider.AliasFor<TLeft>();
            if (joinAlias == null)
                joinAlias = MetadataProvider.AliasFor<TJoin>();

            return InnerJoin(GetJoinFilter(condition.Body as BinaryExpression, leftAlias, joinAlias), joinAlias);
        }

        public SqlSelect<T> InnerJoin<TJoin>(ISqlFilter condition, SqlAlias<TJoin> joinAlias = null)
        {
            Guard.IsNotNull(condition);
            return new SqlSelect<T>(Join(JoinType.Inner, condition, joinAlias));
        }

        public SqlSelect<T> LeftJoin<TLeft, TJoin>(Expression<Func<TLeft, TJoin, bool>> condition,
            SqlAlias<TLeft> leftAlias = null, SqlAlias<TJoin> joinAlias = null)
        {
            Guard.IsNotNull(condition);
            if (leftAlias == null)
                leftAlias = MetadataProvider.AliasFor<TLeft>();
            if (leftAlias == null)
                joinAlias = MetadataProvider.AliasFor<TJoin>();

            return LeftJoin(GetJoinFilter(condition.Body as BinaryExpression, leftAlias, joinAlias), joinAlias);
        }

        public SqlSelect<T> LeftJoin<TJoin>(ISqlFilter condition, SqlAlias<TJoin> joinAlias = null)
        {
            Guard.IsNotNull(condition);
            return new SqlSelect<T>(Join(JoinType.Left, condition, joinAlias));
        }

        public SqlSelect<T> RightJoin<TLeft, TJoin>(Expression<Func<TLeft, TJoin, bool>> condition,
            SqlAlias<TLeft> leftAlias = null, SqlAlias<TJoin> joinAlias = null)
        {
            Guard.IsNotNull(condition);
            if (leftAlias == null)
                leftAlias = MetadataProvider.AliasFor<TLeft>();
            if (joinAlias == null)
                joinAlias = MetadataProvider.AliasFor<TJoin>();

            return RightJoin(GetJoinFilter(condition.Body as BinaryExpression, leftAlias, joinAlias), joinAlias);
        }

        public SqlSelect<T> RightJoin<TJoin>(ISqlFilter condition, SqlAlias<TJoin> joinAlias = null)
        {
            Guard.IsNotNull(condition);
            return new SqlSelect<T>(Join(JoinType.Right, condition, joinAlias));
        }

        public SqlSelect<T> FullJoin<TLeft, TJoin>(Expression<Func<TLeft, TJoin, bool>> condition,
            SqlAlias<TLeft> leftAlias = null, SqlAlias<TJoin> joinAlias = null)
        {
            Guard.IsNotNull(condition);
            if (leftAlias == null)
                leftAlias = MetadataProvider.AliasFor<TLeft>();
            if (joinAlias == null)
                joinAlias = MetadataProvider.AliasFor<TJoin>();

            return FullJoin(GetJoinFilter(condition.Body as BinaryExpression, leftAlias, joinAlias), joinAlias);
        }

        public SqlSelect<T> FullJoin<TJoin>(ISqlFilter condition, SqlAlias<TJoin> joinAlias = null)
        {
            Guard.IsNotNull(condition);
            return new SqlSelect<T>(Join(JoinType.Full, condition, joinAlias));
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
            return new SqlSelect<T>(Info.Where(filter));
        }

        ISqlSelect ISqlSelect.Having(ISqlFilter filter) => Having(filter);
        public SqlSelect<T> Having(ISqlFilter filter)
        {
            return new SqlSelect<T>(Info.Having(filter));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SqlSelectBuilder.SqlFilter;

namespace SqlSelectBuilder
{
    [ContractClass(typeof(ISqlSelectContract))]
    public interface ISqlSelect
    {
        Type EntityType { get; }
        string CommandText { get; }
        IEnumerable<ISqlField> SelectFields { get; }
        IEnumerable<ISqlField> GroupByFields { get; }
        IEnumerable<ISqlField> OrderByFields { get; }
        IEnumerable<ISqlAlias> Aliases { get; }

        void Top(int top, bool topByPercent = false);
        void Distinct(bool isDistinct);

        void AddField(ISqlField field);
        void AddFields<TEntity>(SqlAlias<TEntity> alias = null,
            params Expression<Func<TEntity, object>>[] fields);

        void GroupBy<TEntity>(SqlAlias<TEntity> alias = null,
            params Expression<Func<TEntity, object>>[] fields);

        void OrderBy<TEntity>(SqlAlias<TEntity> alias = null,
            params Expression<Func<TEntity, object>>[] fields);

        void Join<TJoin>(JoinType joinType, ISqlFilter condition, SqlAlias<TJoin> joinAlias = null);

        void Where(ISqlFilter filter);
        void Having(ISqlFilter filter);
    }

    [ContractClassFor(typeof(ISqlSelect))]
    [ExcludeFromCodeCoverage]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal abstract class ISqlSelectContract : ISqlSelect
    {
        public IEnumerable<ISqlAlias> Aliases
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<ISqlAlias>>() != null);
                throw new NotImplementedException();
            }
        }

        public string CommandText
        {
            get
            {
                Contract.Ensures(Contract.Result<string>().IsNotEmpty());
                throw new NotImplementedException();
            }
        }

        public IEnumerable<ISqlField> SelectFields
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<ISqlField>>() != null);
                throw new NotImplementedException();
            }
        }

        public Type EntityType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<ISqlField> GroupByFields
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<ISqlField> OrderByFields
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Distinct(bool isDistinct)
        {
            throw new NotImplementedException();
        }

        public void AddField(ISqlField field)
        {
            Contract.Requires(field != null);
            throw new NotImplementedException();
        }

        public void AddFields<TEntity>(SqlAlias<TEntity> alias = null, params Expression<Func<TEntity, object>>[] fields)
        {
            Contract.Requires(fields != null);
            throw new NotImplementedException();
        }

        public void GroupBy<TEntity>(SqlAlias<TEntity> alias = null, params Expression<Func<TEntity, object>>[] fields)
        {
            Contract.Requires(fields != null);
            throw new NotImplementedException();
        }

        public void OrderBy<TEntity>(SqlAlias<TEntity> alias = null, params Expression<Func<TEntity, object>>[] fields)
        {
            Contract.Requires(fields != null);
            throw new NotImplementedException();
        }

        public void Join<TJoin>(JoinType joinType, ISqlFilter condition, SqlAlias<TJoin> joinAlias = null)
        {
            Contract.Requires(condition != null);
            throw new NotImplementedException();
        }

        public void Where(ISqlFilter filter)
        {
            Contract.Requires(filter != null);
            throw new NotImplementedException();
        }

        public void Having(ISqlFilter filter)
        {
            Contract.Requires(filter != null);
            throw new NotImplementedException();
        }

        public void Top(int top, bool topByPercent = false)
        {
            Contract.Requires(top > 0);
            throw new NotImplementedException();
        }
    }
}

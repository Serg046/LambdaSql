using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GuardExtensions;

namespace SqlSelectBuilder
{
    [ContractClass(typeof(ISqlFilterContract))]
    public interface ISqlFilter
    {
        string Filter { get; }
    }

    [ContractClassFor(typeof(ISqlFilter))]
    [ExcludeFromCodeCoverage]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal abstract class ISqlFilterContract : ISqlFilter
    {
        public string Filter
        {
            get
            {
                Contract.Ensures(Contract.Result<string>().IsNotEmpty());
                throw new NotImplementedException();
            }
        }
    }

    public class SqlFilter<TEntity> : ISqlFilter
    {
        private const string AND = " AND ";
        private const string OR = " OR ";

        internal SqlFilter()
        {
            Filter = null;
        }

        internal SqlFilter(string filter)
        {
            Guard.IsNotEmpty(filter);
            Filter = filter;
        }

        public string Filter { get; }

        public override string ToString() => Filter;

        private static string GetFieldName(LambdaExpression field, ISqlAlias alias)
        {
            Contract.Requires(field != null);
            Contract.Requires(alias != null);
            Contract.Ensures(Contract.Result<string>().IsNotEmpty());
            var fieldName = MetadataProvider.Instance.GetPropertyName(field);
            return alias.Value + "." + fieldName;
        }

        private SqlFilterField<TEntity, TType> AddFilter<TType>(string field, string logicSeparator)
        {
            Contract.Requires(field.IsNotEmpty());
            Contract.Requires(logicSeparator.IsNotEmpty());

            return new SqlFilterField<TEntity, TType>(Filter + logicSeparator, field);
        }

        //----------------------------------

        public static SqlFilterField<TEntity, TType> From<TType>(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilterField<TEntity, TType>>() != null);
            Guard.IsNotNull(field);

            if (alias == null)
                alias = MetadataProvider.Instance.AliasFor<TEntity>();
            return new SqlFilterField<TEntity, TType>(GetFieldName(field, alias));
        }

        public static SqlFilterField<TEntity, TType> From<TType>(ISqlField field)
        {
            Contract.Ensures(Contract.Result<SqlFilterField<TEntity, TType>>() != null);
            Guard.IsNotNull(field);
            return new SqlFilterField<TEntity, TType>(field.ShortView);
        }

        public SqlFilter<T> Cast<T>()
        {
            Contract.Ensures(Contract.Result<SqlFilter<T>>() != null);
            return new SqlFilter<T>(Filter);
        }

        public SqlFilterField<TEntity, TType> And<TType>(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilterField<TEntity, TType>>() != null);
            Guard.IsNotNull(field);
            if (alias == null)
                alias = MetadataProvider.Instance.AliasFor<TEntity>();
            return AddFilter<TType>(GetFieldName(field, alias), AND);
        }

        public SqlFilterField<TEntity, TType> Or<TType>(Expression<Func<TEntity, TType>> field, SqlAlias<TEntity> alias = null)
        {
            Contract.Ensures(Contract.Result<SqlFilterField<TEntity, TType>>() != null);
            Guard.IsNotNull(field);
            if (alias == null)
                alias = MetadataProvider.Instance.AliasFor<TEntity>();
            return AddFilter<TType>(GetFieldName(field, alias), OR);
        }

        public SqlFilterField<TEntity, TType> And<TType>(ISqlField field)
        {
            Contract.Ensures(Contract.Result<SqlFilterField<TEntity, TType>>() != null);
            Guard.IsNotNull(field);
            return AddFilter<TType>(field.ShortView, AND);
        }

        public SqlFilterField<TEntity, TType> Or<TType>(ISqlField field)
        {
            Contract.Ensures(Contract.Result<SqlFilterField<TEntity, TType>>() != null);
            Guard.IsNotNull(field);
            return AddFilter<TType>(field.ShortView, OR);
        }

        public SqlFilter<TEntity> And(ISqlFilter filter)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(filter);
            return new SqlFilter<TEntity>(Filter + AND + filter.Filter);
        }

        public SqlFilter<TEntity> Or(ISqlFilter filter)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(filter);
            return new SqlFilter<TEntity>(Filter + OR + filter.Filter);
        }

        public SqlFilter<TEntity> AndGroup(ISqlFilter filter)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(filter);
            return new SqlFilter<TEntity>($"{Filter}{AND}({filter.Filter})");
        }

        public SqlFilter<TEntity> OrGroup(ISqlFilter filter)
        {
            Contract.Ensures(Contract.Result<SqlFilter<TEntity>>() != null);
            Guard.IsNotNull(filter);
            return new SqlFilter<TEntity>($"{Filter}{OR}({filter.Filter})");
        }
    }
}

using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using GuardExtensions;

namespace SqlSelectBuilder.SqlFilter
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

    public class SqlFilterBase<TEntity> : ISqlFilter
    {
        protected const string AND = " AND ";
        protected const string OR = " OR ";

        internal SqlFilterBase()
        {
            Filter = null;
        }

        internal SqlFilterBase(string filter)
        {
            Guard.IsNotEmpty(filter);
            Filter = filter;
        }

        public string Filter { get; }

        public override string ToString() => Filter;

        protected static SqlAlias<TEntity> CheckAlias(SqlAlias<TEntity> alias)
        {
            return alias ?? MetadataProvider.Instance.AliasFor<TEntity>();
        }

        protected static string GetFieldName(LambdaExpression field, ISqlAlias alias)
        {
            Contract.Requires(field != null);
            Contract.Requires(alias != null);
            Contract.Ensures(Contract.Result<string>().IsNotEmpty());
            var fieldName = MetadataProvider.Instance.GetPropertyName(field);
            return alias.Value + "." + fieldName;
        }
    }
}

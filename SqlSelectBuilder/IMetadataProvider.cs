using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSelectBuilder
{
    [ContractClass(typeof(IAliasManagerContract))]
    public interface IAliasManager
    {
        SqlAlias<TEntity> AliasFor<TEntity>();
    }

    [ContractClass(typeof(IParameterConverterContract))]
    public interface IParameterConverter
    {
        string ParameterToString(object value);
    }

    [ContractClass(typeof(IMetadataProviderContract))]
    public interface IMetadataProvider : IParameterConverter, IAliasManager
    {
        string GetTableName<TEntity>();
        string GetTableName(Type entityType);
        string GetPropertyName(LambdaExpression propertyExpression);
        string GetPropertyName(MemberExpression propertyExpression);
    }

    [ContractClassFor(typeof(IAliasManager))]
    [ExcludeFromCodeCoverage]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal abstract class IAliasManagerContract : IAliasManager
    {
        public SqlAlias<TEntity> AliasFor<TEntity>()
        {
            Contract.Ensures(Contract.Result<SqlAlias<TEntity>>() != null);
            throw new NotImplementedException();
        }
    }

    [ContractClassFor(typeof(IParameterConverter))]
    [ExcludeFromCodeCoverage]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal abstract class IParameterConverterContract : IParameterConverter
    {
        public string ParameterToString(object value)
        {
            Contract.Requires(value != null);
            Contract.Ensures(Contract.Result<string>().IsNotEmpty());
            throw new NotImplementedException();
        }
    }

    [ContractClassFor(typeof(IMetadataProvider))]
    [ExcludeFromCodeCoverage]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal abstract class IMetadataProviderContract : IMetadataProvider
    {
        public string ParameterToString(object value)
        {
            throw new NotImplementedException();
        }

        public SqlAlias<TEntity> AliasFor<TEntity>()
        {
            throw new NotImplementedException();
        }

        public string GetTableName<TEntity>()
        {
            Contract.Ensures(Contract.Result<string>().IsNotEmpty());
            throw new NotImplementedException();
        }

        public string GetTableName(Type entityType)
        {
            Contract.Requires(entityType != null);
            Contract.Ensures(Contract.Result<string>().IsNotEmpty());
            throw new NotImplementedException();
        }

        public string GetPropertyName(LambdaExpression propertyExpression)
        {
            Contract.Requires(propertyExpression != null);
            Contract.Ensures(Contract.Result<string>().IsNotEmpty());
            throw new NotImplementedException();
        }

        public string GetPropertyName(MemberExpression propertyExpression)
        {
            Contract.Requires(propertyExpression != null);
            Contract.Ensures(Contract.Result<string>().IsNotEmpty());
            throw new NotImplementedException();
        }
    }
}

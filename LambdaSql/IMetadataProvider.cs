using System;
using System.Linq.Expressions;

namespace LambdaSql
{
    public interface IAliasManager
    {
        SqlAlias<TEntity> AliasFor<TEntity>();
    }

    public interface IParameterConverter
    {
        string ParameterToString(object value);
    }

    public interface IMetadataProvider : IParameterConverter, IAliasManager
    {
        string GetTableName<TEntity>();
        string GetTableName(Type entityType);
        string GetPropertyName(LambdaExpression propertyExpression);
        string GetPropertyName(MemberExpression propertyExpression);
    }
}

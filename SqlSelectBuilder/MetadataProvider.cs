using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GuardExtensions;

namespace SqlSelectBuilder
{
    public class MetadataProvider : IMetadataProvider
    {
        private SqlAliasContainer _aliasContainer;

        static MetadataProvider()
        {
            Instance = new MetadataProvider();
        }

        private MetadataProvider()
        {
            _aliasContainer = new SqlAliasContainer();
        }

        public static IMetadataProvider Instance { get; private set; }

        public static void Initialize(SqlAliasContainerBuilder aliasContainerBuilder)
        {
            var provider = Instance as MetadataProvider;
            if (provider == null)
                throw new InvalidOperationException("Supports only the default metadata provider");
            Guard.IsNotNull(aliasContainerBuilder);
            Guard.IsPositive(aliasContainerBuilder.RegisteredAliases.Count);
            provider._aliasContainer = new SqlAliasContainer(aliasContainerBuilder.RegisteredAliases);
        }

        public static void Initialize(IMetadataProvider metadataProvider)
        {
            Guard.IsNotNull(metadataProvider);
            Instance = metadataProvider;
        }

        public string GetTableName<TEntity>()
        {
            return GetTableName(typeof(TEntity));
        }

        public string GetTableName(Type entityType)
        {
            Guard.IsNotNull(entityType);
            return entityType.Name;
        }

        public string GetPropertyName(LambdaExpression propertyExpression)
        {
            Guard.IsNotNull(propertyExpression);
            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression != null)
            {
                return GetPropertyName(memberExpression);
            }
            else
            {
                var unary = propertyExpression.Body as UnaryExpression;
                if (unary != null && unary.NodeType == ExpressionType.Convert && unary.Operand is MemberExpression)
                {
                    return GetPropertyName((MemberExpression)unary.Operand);
                }
            }
            throw new InvalidOperationException();
        }

        public string GetPropertyName(MemberExpression memberExpression)
        {
            Guard.IsNotNull(memberExpression);
            return memberExpression.Member.Name;
        }

        public string ParameterToString(object value)
        {
            Guard.IsNotNull(value);
            if (value is int)
                return ((int)value).ToString();
            if (value is string)
            {
                var val = value.ToString();
                if (val.Length > 0)
                {
                    return $"'{val}'";
                }
                throw new NotSupportedException($"The value is empty");
            }
            throw new NotSupportedException($"Type {value.GetType().FullName} is not supported as parameter");
        }

        public SqlAlias<TEntity> AliasFor<TEntity>()
        {
            return _aliasContainer.For<TEntity>();
        }
    }
}

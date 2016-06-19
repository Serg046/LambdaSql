using System;
using System.Linq.Expressions;
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

        protected MetadataProvider()
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

        public virtual string GetTableName<TEntity>()
        {
            return GetTableName(typeof(TEntity));
        }

        public virtual string GetTableName(Type entityType)
        {
            Guard.IsNotNull(entityType);
            return entityType.Name;
        }

        public virtual string GetPropertyName(LambdaExpression propertyExpression)
        {
            Guard.IsNotNull(propertyExpression);
            return GetPropertyName(LibHelper.GetMemberExpression(propertyExpression));
        }

        public virtual string GetPropertyName(MemberExpression memberExpression)
        {
            Guard.IsNotNull(memberExpression);
            return memberExpression.Member.Name;
        }

        public virtual string ParameterToString(object value)
        {
            Guard.IsNotNull(value);

            var paramType = Nullable.GetUnderlyingType(value.GetType()) ?? value.GetType();

            if (paramType == typeof(int))
                return ((int)value).ToString();
            if (paramType == typeof(string))
            {
                var val = value.ToString();
                if (val.Length > 0)
                {
                    return $"'{val}'";
                }
                throw new NotSupportedException($"The value is empty");
            }
            if (paramType == typeof(bool))
                return (bool)value ? "1" : "0";
            throw new NotSupportedException($"Type {value.GetType().FullName} is not supported as parameter");
        }

        public virtual SqlAlias<TEntity> AliasFor<TEntity>()
        {
            return _aliasContainer.For<TEntity>();
        }
    }
}

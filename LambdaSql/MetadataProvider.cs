using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

namespace LambdaSql
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
            if (aliasContainerBuilder == null) throw new ArgumentNullException(nameof(aliasContainerBuilder));
            if (aliasContainerBuilder.RegisteredAliases?.Any() != true) throw new ArgumentException("RegisteredAliases contains no elements");
            var provider = Instance as MetadataProvider;
            if (provider == null) throw new InvalidOperationException("The method supports only the default metadata provider");
            provider._aliasContainer = new SqlAliasContainer(aliasContainerBuilder.RegisteredAliases);
        }

        public static void Initialize(IMetadataProvider metadataProvider)
        {
            Instance = metadataProvider ?? throw new ArgumentNullException(nameof(metadataProvider));
        }

        public virtual string GetTableName<TEntity>()
        {
            return GetTableName(typeof(TEntity));
        }

        public virtual string GetTableName(Type entityType)
        {
            if (entityType == null) throw new ArgumentNullException(nameof(entityType));
            return entityType.Name;
        }

        public virtual string GetPropertyName(LambdaExpression propertyExpression)
        {
            if (propertyExpression == null) throw new ArgumentNullException(nameof(propertyExpression));
            return GetPropertyName(LibHelper.GetMemberExpression(propertyExpression));
        }

        public virtual string GetPropertyName(MemberExpression memberExpression)
        {
            if (memberExpression == null) throw new ArgumentNullException(nameof(memberExpression));
            return memberExpression.Member.Name;
        }

        public DbParameter CreateDbParameter() => new SqlParameter();

        public virtual string ParameterToString(object value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            switch (value)
            {
                case bool flag: return flag ? "1" : "0";
            }

            return value.ToString();
        }

        public virtual SqlAlias<TEntity> AliasFor<TEntity>()
        {
            return _aliasContainer.For<TEntity>();
        }
    }
}

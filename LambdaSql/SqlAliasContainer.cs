using System;
using System.Collections.Generic;
using System.Linq;

namespace LambdaSql
{
    internal class SqlAliasContainer
    {
        private readonly Dictionary<Type, ISqlAlias> _aliases;
        private readonly object _syncObject = new object();

        public SqlAliasContainer()
        {
            _aliases = new Dictionary<Type, ISqlAlias>();
        }

        public SqlAliasContainer(Dictionary<Type, ISqlAlias> aliases)
        {
            _aliases = aliases;
        }

        public SqlAlias<TEntity> For<TEntity>()
        {
            lock (_syncObject)
            {
                return TryGetAlias<TEntity>() ?? InitAlias<TEntity>();
            }
        }

        private SqlAlias<TEntity> TryGetAlias<TEntity>()
        {
            var entityType = typeof(TEntity);
            return _aliases.ContainsKey(entityType)
                ? _aliases[entityType] as SqlAlias<TEntity>
                : null;
        }

        private SqlAlias<TEntity> InitAlias<TEntity>()
        {
            var entityType = typeof (TEntity);
            var alias = new SqlAlias<TEntity>(GenerateAliasName(entityType));
            _aliases.Add(entityType, alias);
            return alias;
        }

        private string GenerateAliasName(Type entityType)
        {
            if (entityType == null) throw new ArgumentNullException(nameof(entityType));

            var alias = MetadataProvider.Instance.GetTableName(entityType);
            if (alias.Length > 2)
                alias = alias.Substring(0, 2).ToLower();
            var existingAlias = _aliases.SingleOrDefault(a => a.Value.Value == alias);
            if (existingAlias.Value != null)
            {
                throw new DuplicateAliasException(
                    $"Please register aliases for the following types: {existingAlias.Key.FullName}, {entityType.FullName}. Use SqlAliasContainerBuilder.");
            }
            return alias;
        }
    }
}

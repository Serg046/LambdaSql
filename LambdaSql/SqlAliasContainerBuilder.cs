using System;
using System.Collections.Generic;

namespace LambdaSql
{
    public class SqlAliasContainerBuilder
    {
        private readonly Dictionary<Type, ISqlAlias> _aliases = new Dictionary<Type, ISqlAlias>();

        public void Register<T>(string alias)
        {
            var entityType = typeof(T);
            if (_aliases.ContainsKey(entityType))
                throw new ArgumentException($"{entityType.FullName} is already registered");
            _aliases.Add(entityType, new SqlAlias<T>(alias));
        }

        public Dictionary<Type, ISqlAlias> RegisteredAliases => _aliases;
    }
}

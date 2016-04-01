using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuardExtensions;

namespace SqlSelectBuilder.SqlFilter
{
    public interface ISqlFilterItem
    {
        string ToString(bool withoutAliases = false);
    }

    public class SqlFilterItem : ISqlFilterItem
    {
        private string _expression = null;
        private object[] _args = null;

        public SqlFilterItem(ISqlField sqlField)
        {
            Guard.IsNotNull(sqlField);
            SqlField = sqlField;
        }

        public ISqlField SqlField { get; }

        internal SqlFilterItem SetExpression(string expression, params object[] args)
        {
            if (_expression != null || _args != null)
                throw new InvalidOperationException("Expression is already initialized");

            _args = args;
            _expression = expression;
            return this;
        }

        public override string ToString()
        {
            return string.Format(_expression, _args);
        }

        public string ToString(bool withoutAliases)
        {
            if (withoutAliases && _args != null && _args.Length > 0)
            {
                var args = new object[_args.Length];
                for (var i = 0; i < _args.Length; i++)
                {
                    var field = _args[i] as ISqlField;
                    args[i] = field != null ? field.Name : _args[i];
                }
                return string.Format(_expression, args);
            }
            return ToString();
        }
    }

    public static class SqlFilterItems
    {
        public static ConstSqlFilterItem And = new ConstSqlFilterItem(" AND ");
        public static ConstSqlFilterItem Or = new ConstSqlFilterItem(" OR ");

        public static ConstSqlFilterItem Build(string value) => new ConstSqlFilterItem(value);
    }

    public class ConstSqlFilterItem : ISqlFilterItem
    {
        private readonly string _value;

        public ConstSqlFilterItem(string value)
        {
            _value = value;
        }

        public string ToString(bool withoutAliases = false) => _value;
    }
}

using System.Data;
using System.Data.Common;
using System.Linq;
using LambdaSql.Field;

namespace LambdaSql.Filter.SqlFilterItem
{
    internal abstract class SqlFilterParameter
    {
        private readonly SqlFilterConfiguration _configuration;

        private SqlFilterParameter(SqlFilterConfiguration configuration)
        {
            _configuration = configuration;
        }

        public abstract string Value { get; }
        public abstract DbParameter Parameter { get; }

        public static SqlFilterParameter Create(SqlFilterConfiguration configuration, ISqlField sqlField)
            => new SqlFieldParameter(configuration, sqlField);

        public static SqlFilterParameter Create(SqlFilterConfiguration configuration, object paramValue)
        {
            var dbParameter = MetadataProvider.Instance.CreateDbParameter();
            dbParameter.Value = paramValue;
            return new DbParamParameter(configuration, dbParameter);
        }

        public override string ToString() => Value;

        //-----------------------------------------------------------------------------------

        private class SqlFieldParameter : SqlFilterParameter
        {
            private readonly ISqlField _sqlField;

            public SqlFieldParameter(SqlFilterConfiguration configuration, ISqlField sqlField) : base(configuration)
            {
                _sqlField = sqlField;
            }

            public override DbParameter Parameter => null;

            public override string Value
            {
                get
                {
                    var field = _sqlField.Clone();
                    if (_configuration.WithoutAliases)
                        field.Alias = null;
                    return field.ShortString;
                }
            }
        }

        private class DbParamParameter : SqlFilterParameter
        {
            private static readonly DbType[] _quotedParameterTypes = {
                DbType.AnsiString, DbType.Date,
                DbType.DateTime, DbType.Guid, DbType.String,
                DbType.AnsiStringFixedLength, DbType.StringFixedLength
            };

            private readonly DbParameter _dbParameter;

            public DbParamParameter(SqlFilterConfiguration configuration, DbParameter dbParameter)
                : base(configuration)
            {
                _dbParameter = dbParameter;
            }

            public override DbParameter Parameter
                => _configuration.WithoutParameters ? null : _dbParameter;

            public override string Value
                => _configuration.WithoutParameters
                    ? _quotedParameterTypes.Contains(_dbParameter.DbType) ? $"'{_dbParameter.Value}'" : _dbParameter.Value.ToString()
                    : _dbParameter.ParameterName;
        }
    }
}

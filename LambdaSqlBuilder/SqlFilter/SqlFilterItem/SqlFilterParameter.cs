using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace LambdaSqlBuilder.SqlFilter.SqlFilterItem
{
    internal abstract class SqlFilterParameter
    {
        private readonly SqlFilterConfiguration _configuration;

        private SqlFilterParameter(SqlFilterConfiguration configuration)
        {
            _configuration = configuration;
        }

        public abstract string Value { get; }
        public abstract SqlParameter Parameter { get; }

        public static SqlFilterParameter Create(SqlFilterConfiguration configuration, ISqlField sqlField)
            => new SqlFieldParameter(configuration, sqlField);

        public static SqlFilterParameter Create(SqlFilterConfiguration configuration, object paramValue)
            => new DbParamParameter(configuration, new SqlParameter { Value = paramValue });

        public override string ToString() => Value;

        //-----------------------------------------------------------------------------------

        private class SqlFieldParameter : SqlFilterParameter
        {
            private readonly ISqlField _sqlField;

            public SqlFieldParameter(SqlFilterConfiguration configuration, ISqlField sqlField) : base(configuration)
            {
                _sqlField = sqlField;
            }

            public override SqlParameter Parameter => null;

            public override string Value
            {
                get
                {
                    var field = _sqlField.Clone();
                    field.AsAlias = null;
                    if (_configuration.WithoutAliases)
                        field.Alias = null;
                    return field.ShortView;
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

            private readonly SqlParameter _dbParameter;

            public DbParamParameter(SqlFilterConfiguration configuration, SqlParameter dbParameter)
                : base(configuration)
            {
                _dbParameter = dbParameter;
            }

            public override SqlParameter Parameter
                => _configuration.WithoutParameters ? null : _dbParameter;

            public override string Value
                => _configuration.WithoutParameters
                    ? _quotedParameterTypes.Contains(_dbParameter.DbType) ? $"'{_dbParameter.Value}'" : _dbParameter.Value.ToString()
                    : _dbParameter.ParameterName;
        }
    }
}

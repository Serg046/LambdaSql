using System.Data.SqlClient;

namespace LambdaSql.Filter
{
    public interface ISqlFilter
    {
        string RawSql { get; }
        string ParametricSql { get; }
        SqlParameter[] Parameters { get; }
    }
}

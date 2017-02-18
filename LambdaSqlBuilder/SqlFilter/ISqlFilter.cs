using System.Data.SqlClient;

namespace LambdaSqlBuilder.SqlFilter
{
    public interface ISqlFilter
    {
        string RawSql { get; }
        string ParametricSql { get; }
        SqlParameter[] Parameters { get; }
    }
}

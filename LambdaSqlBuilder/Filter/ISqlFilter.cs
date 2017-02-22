using System.Data.SqlClient;

namespace LambdaSqlBuilder.Filter
{
    public interface ISqlFilter
    {
        string RawSql { get; }
        string ParametricSql { get; }
        SqlParameter[] Parameters { get; }
    }
}

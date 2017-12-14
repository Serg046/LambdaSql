using System.Data.Common;

namespace LambdaSql.Filter
{
    public interface ISqlFilter
    {
        string RawSql { get; }
        string ParametricSql { get; }
        DbParameter[] Parameters { get; }
        ISqlFilter WithParameterPrefix(string prefix);
    }
}

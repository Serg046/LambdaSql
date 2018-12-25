using LambdaSql.Filter;
using LambdaSql.Filter.SqlFilterItem;
using Xunit;

namespace LambdaSql.UnitTests.Filter
{
    public class SqlFilterParameterTest
    {
        [Theory]
        [InlineData(true, "1")]
        [InlineData(false, "0")]
        public void Create_BooleanParam_ReturnsBinaryNumber(bool? param, string expected)
        {
            var configuration = new SqlFilterConfiguration {WithoutParameters = true};

            var filterParam = SqlFilterParameter.Create(configuration, param);

            Assert.Equal(expected, filterParam.Value);
        }
    }
}

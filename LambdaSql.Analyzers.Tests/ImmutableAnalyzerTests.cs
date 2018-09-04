using System;
using LambdaSql.Analyzers.Tests.Verifiers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace LambdaSql.Analyzers.Tests
{
    public class ImmutableAnalyzerTests : DiagnosticVerifier
    {
        [Theory]
        [InlineData("new SqlSelect<int>().Distinct()", "LambdaSql.SqlSelect<int>", "Distinct")]
        [InlineData("new SqlSelect<int>().Distinct().Distinct(false)", "LambdaSql.SqlSelect<int>", "Distinct")]
        public void InvalidExpression_DiagnosticIsReported(string expression, string type, string method)
        {
var test = $@"
namespace LambdaSql.Analyzers.Tests
{{
    class TestType
    {{
        void TestMethod()
        {{
            {expression}
        }}
    }}
}}";
            var expected = new DiagnosticResult
            {
                Id = "LSql1000",
                Message = String.Format(ImmutableAnalyzer.MESSAGE_FORMAT, type, method),
                Severity = DiagnosticSeverity.Error,
                Locations = new[] {new DiagnosticResultLocation("Test0.cs", 8, 13)}
            };
            VerifyCSharpDiagnostic(test, expected);
        }

        [Theory]
        [InlineData("var qry = new SqlSelect<int>().Distinct()")]
        [InlineData("var qry = new SqlSelect<int>().Distinct().Distinct(false)")]
        public void ValidExpression_DiagnosticIsReported(string expression)
        {
            var test = $@"
namespace LambdaSql.Analyzers.Tests
{{
    class TestType
    {{
        void TestMethod()
        {{
            {expression}
        }}
    }}
}}";

            VerifyCSharpDiagnostic(test);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new ImmutableAnalyzer();
    }
}

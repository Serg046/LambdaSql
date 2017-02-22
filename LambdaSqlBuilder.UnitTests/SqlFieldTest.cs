using LambdaSqlBuilder.Field;
using LambdaSqlBuilder.UnitTests.Entities;
using Xunit;

namespace LambdaSqlBuilder.UnitTests
{
    public class SqlFieldTest
    {
        [Fact]
        public void From_StringViewIsValid()
        {
            Assert.Equal("pe.Name AS TestAlias", SqlField<Person>.From(p => p.Name, "TestAlias").ToString());
            var alias = new SqlAlias<Passport>("t");
            Assert.Equal("t.Number AS Passp", SqlField<Passport>.From(alias, p => p.Number, "Passp").ToString());
        }

        [Fact]
        public void Min_StringViewIsValid()
        {
            Assert.Equal("MIN(pe.Name) AS TestAlias", SqlField<Person>.Min(p => p.Name, "TestAlias").ToString());
            var alias = new SqlAlias<Passport>("t");
            Assert.Equal("MIN(t.Number) AS Passp", SqlField<Passport>.Min(alias, p => p.Number, "Passp").ToString());
        }

        [Fact]
        public void Max_StringViewIsValid()
        {
            Assert.Equal("MAX(pe.Name) AS TestAlias", SqlField<Person>.Max(p => p.Name, "TestAlias").ToString());
            var alias = new SqlAlias<Passport>("t");
            Assert.Equal("MAX(t.Number) AS Passp", SqlField<Passport>.Max(alias, p => p.Number, "Passp").ToString());
        }

        [Fact]
        public void Avg_StringViewIsValid()
        {
            Assert.Equal("AVG(pe.Name) AS TestAlias", SqlField<Person>.Avg(p => p.Name, "TestAlias").ToString());
            var alias = new SqlAlias<Passport>("t");
            Assert.Equal("AVG(t.Number) AS Passp", SqlField<Passport>.Avg(alias, p => p.Number, "Passp").ToString());
        }

        [Fact]
        public void Sum_StringViewIsValid()
        {
            Assert.Equal("SUM(pe.Name) AS TestAlias", SqlField<Person>.Sum(p => p.Name, "TestAlias").ToString());
            var alias = new SqlAlias<Passport>("t");
            Assert.Equal("SUM(t.Number) AS Passp", SqlField<Passport>.Sum(alias, p => p.Number, "Passp").ToString());
        }

        [Fact]
        public void Count_StringViewIsValid()
        {
            Assert.Equal("COUNT(pe.Name) AS TestAlias", SqlField<Person>.Count(p => p.Name, "TestAlias").ToString());
            var alias = new SqlAlias<Passport>("t");
            Assert.Equal("COUNT(t.Number) AS Passp", SqlField<Passport>.Count(alias, p => p.Number, "Passp").ToString());
        }
    }
}

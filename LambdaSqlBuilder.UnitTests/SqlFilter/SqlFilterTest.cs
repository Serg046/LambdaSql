using LambdaSqlBuilder.SqlFilter;
using LambdaSqlBuilder.UnitTests.Entities;
using Xunit;

namespace LambdaSqlBuilder.UnitTests.SqlFilter
{
    public class SqlFilterTest
    {
        [Fact]
        public void And()
        {
            var filter = SqlFilter<Person>.From(m => m.Id).EqualTo(5)
                .And(m => m.Name).EqualTo("Sergey");

            Assert.Equal("pe.Id = 5 AND pe.Name = 'Sergey'", filter.RawSql);
        }

        [Fact]
        public void Or()
        {
            var filter = SqlFilter<Person>.From(m => m.Id).EqualTo(5)
                .Or(m => m.Name).EqualTo("Sergey");

            Assert.Equal("pe.Id = 5 OR pe.Name = 'Sergey'", filter.RawSql);
        }

        [Fact]
        public void UserAliasesWorkCorrectly()
        {
            var alias1 = new SqlAlias<Person>("per1");
            var alias2 = new SqlAlias<Person>("per2");

            var andFilter = SqlFilter<Person>.From(m => m.Id, alias1).EqualTo(5)
                .And(m => m.Name, alias2).EqualTo("Sergey");
            var orFilter = SqlFilter<Person>.From(m => m.Id, alias2).EqualTo(5)
                .Or(m => m.Name, alias1).EqualTo("Sergey");

            Assert.Equal("per1.Id = 5 AND per2.Name = 'Sergey'", andFilter.RawSql);
            Assert.Equal("per2.Id = 5 OR per1.Name = 'Sergey'", orFilter.RawSql);
        }

        [Fact]
        public void FilterIsImmutable()
        {
            var filter = SqlFilter<Person>.From(m => m.Id).EqualTo(5);

            Assert.Equal("pe.Id = 5 AND pe.Name = 'Sergey'", filter.And(m => m.Name).EqualTo("Sergey").RawSql);
            Assert.Equal("pe.Id = 5", filter.RawSql);
        }

        [Fact]
        public void TestFiltersFromDifferentTables()
        {
            var filter = SqlFilter<Person>.From(m => m.Id).EqualTo(5)
                .And(m => m.Name).EqualTo("Sergey");
            var passportFilter = SqlFilter<Passport>.From(p => p.PersonId).EqualTo(5)
                .Or(p => p.PersonId).IsNull();

            var joinedFilter1 = filter.And(passportFilter);
            var joinedFilter2 = passportFilter.Or(filter);

            Assert.Equal("pe.Id = 5 AND pe.Name = 'Sergey' AND pa.PersonId = 5 OR pa.PersonId IS NULL", joinedFilter1.RawSql);
            Assert.Equal("pa.PersonId = 5 OR pa.PersonId IS NULL OR pe.Id = 5 AND pe.Name = 'Sergey'", joinedFilter2.RawSql);

            Assert.Equal("pe.Id = 5 AND pe.Name = 'Sergey'", filter.RawSql);
            Assert.Equal("pa.PersonId = 5 OR pa.PersonId IS NULL", passportFilter.RawSql);
        }

        [Fact]
        public void AddingFilterGroupIsCorrect()
        {
            var filter = SqlFilter<Person>.From(m => m.Id).EqualTo(5)
                .AndGroup
                (
                    SqlFilter<Passport>.From(p => p.PersonId).EqualTo(5).And(p => p.Number).IsNotNull()
                        .OrGroup
                        (
                            SqlFilter<Person>.From(p => p.Name).EqualTo("Sergey").Or(p => p.Name).Like("%exception%")
                        )
                );

            Assert.Equal(
                "pe.Id = 5 AND (pa.PersonId = 5 AND pa.Number IS NOT NULL OR (pe.Name = 'Sergey' OR pe.Name LIKE '%exception%'))",
                filter.RawSql);
        }

        [Fact]
        public void WithoutAliases()
        {
            var filter = SqlFilter<Person>.From(m => m.Id).EqualTo(5)
                .And(m => m.Name).EqualTo("Sergey");

            Assert.Equal("Id = 5 AND Name = 'Sergey'", filter.WithoutAliases().RawSql);
            Assert.Equal("pe.Id = 5 AND pe.Name = 'Sergey'", filter.RawSql);
        }

        [Fact]
        public void WithAliases()
        {
            var filter = SqlFilter<Person>.From(m => m.Id).EqualTo(5)
                .And(m => m.Name).EqualTo("Sergey");
            var filterWithoutAliases = filter.WithoutAliases();

            Assert.Equal("Id = 5 AND Name = 'Sergey'", filterWithoutAliases.RawSql);
            Assert.Equal("pe.Id = 5 AND pe.Name = 'Sergey'", filter.RawSql);
            Assert.Equal("pe.Id = 5 AND pe.Name = 'Sergey'", filterWithoutAliases.WithAliases().RawSql);
        }
    }
}

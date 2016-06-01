using SqlSelectBuilder;
using Tests.Entities;
using Xunit;

namespace Tests
{
    public class SqlFilterTest
    {
        [Fact]
        public void And()
        {
            var filter = SqlFilter<Person>.From(m => m.Id).EqualTo(5)
                .And(m => m.Name).EqualTo("Sergey");

            Assert.Equal("pe.Id = 5 AND pe.Name = 'Sergey'", filter.Filter);
        }

        [Fact]
        public void Or()
        {
            var filter = SqlFilter<Person>.From(m => m.Id).EqualTo(5)
                .Or(m => m.Name).EqualTo("Sergey");

            Assert.Equal("pe.Id = 5 OR pe.Name = 'Sergey'", filter.Filter);
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

            Assert.Equal("per1.Id = 5 AND per2.Name = 'Sergey'", andFilter.Filter);
            Assert.Equal("per2.Id = 5 OR per1.Name = 'Sergey'", orFilter.Filter);
        }

        [Fact]
        public void FilterIsImmutable()
        {
            var filter = SqlFilter<Person>.From(m => m.Id).EqualTo(5);

            Assert.Equal("pe.Id = 5 AND pe.Name = 'Sergey'", filter.And(m => m.Name).EqualTo("Sergey").Filter);
            Assert.Equal("pe.Id = 5", filter.Filter);
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

            Assert.Equal("pe.Id = 5 AND pe.Name = 'Sergey' AND pa.PersonId = 5 OR pa.PersonId IS NULL", joinedFilter1.Filter);
            Assert.Equal("pa.PersonId = 5 OR pa.PersonId IS NULL OR pe.Id = 5 AND pe.Name = 'Sergey'", joinedFilter2.Filter);

            Assert.Equal("pe.Id = 5 AND pe.Name = 'Sergey'", filter.Filter);
            Assert.Equal("pa.PersonId = 5 OR pa.PersonId IS NULL", passportFilter.Filter);
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
                filter.Filter);
        }

        [Fact]
        public void WithoutAliases()
        {
            var filter = SqlFilter<Person>.From(m => m.Id).EqualTo(5)
                .And(m => m.Name).EqualTo("Sergey");

            Assert.Equal("Id = 5 AND Name = 'Sergey'", filter.WithoutAliases().Filter);
            Assert.Equal("pe.Id = 5 AND pe.Name = 'Sergey'", filter.Filter);
        }

        [Fact]
        public void WithAliases()
        {
            var filter = SqlFilter<Person>.From(m => m.Id).EqualTo(5)
                .And(m => m.Name).EqualTo("Sergey");
            var filterWithoutAliases = filter.WithoutAliases();

            Assert.Equal("Id = 5 AND Name = 'Sergey'", filterWithoutAliases.Filter);
            Assert.Equal("pe.Id = 5 AND pe.Name = 'Sergey'", filter.Filter);
            Assert.Equal("pe.Id = 5 AND pe.Name = 'Sergey'", filterWithoutAliases.WithAliases().Filter);
        }
    }
}

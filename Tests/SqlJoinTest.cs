using SqlSelectBuilder;
using SqlSelectBuilder.SqlFilter;
using Tests.Entities;
using Xunit;

namespace Tests
{
    public class SqlJoinTest
    {
        [Fact]
        public void InnerJoin()
        {
            var alias = new SqlAlias<Passport>("pas");
            var expected =
@"INNER JOIN
    Passport pas ON pas.PersonId = pe.Id";
            Assert.Equal(expected, new SqlJoin<Passport>(JoinType.Inner,
                SqlFilter<Passport>.From(p => p.PersonId, alias).EqualTo<Person>(p => p.Id), alias).ToString());
        }

        [Fact]
        public void InnerJoinWithLongFilter()
        {
            var alias = new SqlAlias<Passport>("pas");
            var expected =
@"INNER JOIN
    Passport pas ON pas.PersonId = pe.Id AND pa.Number IS NOT NULL";
            Assert.Equal(expected,
                new SqlJoin<Passport>(JoinType.Inner,
                    SqlFilter<Passport>.From(p => p.PersonId, alias).EqualTo<Person>(p => p.Id)
                        .And(p => p.Number).IsNotNull(), alias).ToString());
        }

        [Fact]
        public void LeftJoin()
        {
            var alias = new SqlAlias<Passport>("pas");
            var expected = 
@"LEFT JOIN
    Passport pas ON pe.Id = pas.PersonId";
            Assert.Equal(expected, new SqlJoin<Passport>(JoinType.Left,
                SqlFilter<Person>.From(p => p.Id).EqualTo<Passport>(p => p.PersonId, alias), alias).ToString());
        }

        public void RightJoin()
        {
            var alias = new SqlAlias<Passport>("pas");
            var expected =
@"RIGHT JOIN
    Passport pas ON pe.Id = pas.PersonId";
            Assert.Equal(expected, new SqlJoin<Passport>(JoinType.Right,
                SqlFilter<Person>.From(p => p.Id).EqualTo<Passport>(p => p.PersonId, alias), alias).ToString());
        }

        public void FullJoin()
        {
            var alias = new SqlAlias<Passport>("pas");
            var expected = 
@"FULL JOIN
    Passport pas ON pe.Id = pas.PersonId";
            Assert.Equal(expected, new SqlJoin<Passport>(JoinType.Full,
                SqlFilter<Person>.From(p => p.Id).EqualTo<Passport>(p => p.PersonId, alias), alias).ToString());
        }
    }
}

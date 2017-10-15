using LambdaSql.Field;
using LambdaSql.Filter;
using LambdaSql.UnitTests.Entities;
using Xunit;

namespace LambdaSql.UnitTests
{
    public class SqlSelectTest
    {
        [Fact]
        public void IncorrectAliasThrowsException()
        {
            var select = new SqlSelect<Person>()
                .InnerJoin<Person, Passport>((person, passport) => person.Id == passport.PersonId)
                .AddFields(SqlField<Person>.Count(p => p.Id, "pa"));
            Assert.Throws<IncorrectAliasException>(() => { var cmd = select.CommandText; });
        }

        [Fact]
        public void SimpleSelect()
        {
            var select = new SqlSelect<Person>();

            var expected =
@"SELECT
    *
FROM
    Person pe";
            Assert.Equal(expected, select.CommandText);
        }

        [Fact]
        public void DistinctAndTop()
        {
            var select = new SqlSelect<Person>()
                .AddFields(p => p.Id)
                .Distinct()
                .Top(5);

            var expected =
@"SELECT
    DISTINCT TOP 5 pe.Id
FROM
    Person pe";
            Assert.Equal(expected, select.CommandText);
        }

        [Fact]
        public void Where()
        {
            var select = new SqlSelect<Person>()
                .AddFields(p => p.Id)
                .Where(SqlFilter<Person>.From(p => p.LastName).IsNotNull());

            var expected =
@"SELECT
    pe.Id
FROM
    Person pe
WHERE
    pe.LastName IS NOT NULL";
            Assert.Equal(expected, select.CommandText);
        }

        [Fact]
        public void GroupBy()
        {
            var select = new SqlSelect<Person>()
                .AddFields(SqlField<Person>.Count(p => p.Id))
                .GroupBy(p => p.LastName);

            var expected =
@"SELECT
    COUNT(pe.Id)
FROM
    Person pe
GROUP BY
    pe.LastName";
            Assert.Equal(expected, select.CommandText);
        }

        [Fact]
        public void Having()
        {
            var select = new SqlSelect<Person>()
                .AddFields(SqlField<Person>.Count(p => p.Id))
                .GroupBy(p => p.LastName)
                .Having(SqlFilter<Person>.From<int>(SqlField<Person>.Count(p => p.Id)).GreaterThan(2));

            var expected =
@"SELECT
    COUNT(pe.Id)
FROM
    Person pe
GROUP BY
    pe.LastName
HAVING
    COUNT(pe.Id) > 2";
            Assert.Equal(expected, select.CommandText);
        }

        [Fact]
        public void OrderBy()
        {
            var select = new SqlSelect<Person>()
                .Where(SqlFilter<Person>.From(p => p.LastName).IsNotNull())
                .OrderBy(p => p.Id);

            var expected =
@"SELECT
    *
FROM
    Person pe
WHERE
    pe.LastName IS NOT NULL
ORDER BY
    pe.Id";
            Assert.Equal(expected, select.CommandText);
        }

        [Fact]
        public void Joins()
        {
            var joinByLambdaQry = new SqlSelect<Person>()
                .InnerJoin<Person, Passport>((person, passport) => person.Id == passport.PersonId);
            var joinByFilterQry = new SqlSelect<Person>()
                .InnerJoin<Passport>(SqlFilter<Person>.From(p => p.Id).EqualTo<Passport>(p => p.PersonId));

            var expected =
@"SELECT
    *
FROM
    Person pe
INNER JOIN
    Passport pa ON pe.Id = pa.PersonId";
            Assert.Equal(expected, joinByLambdaQry.CommandText);
            Assert.Equal(expected, joinByFilterQry.CommandText);
        }

        [Fact]
        public void LambdaJoinWithInvertOrder()
        {
            var joinByLambdaQry = new SqlSelect<Person>()
                .InnerJoin<Person, Passport>((person, passport) => passport.PersonId == person.Id);

            var expected =
@"SELECT
    *
FROM
    Person pe
INNER JOIN
    Passport pa ON pa.PersonId = pe.Id";
            Assert.Equal(expected, joinByLambdaQry.CommandText);
        }

        [Fact]
        public void LambdaJoinWithNullableType()
        {
            var joinByLambdaQry = new SqlSelect<Person>()
                .InnerJoin<Person, Passport>((person, passport) => person.Id == passport.NullablePersonId);

            var expected =
@"SELECT
    *
FROM
    Person pe
INNER JOIN
    Passport pa ON pe.Id = pa.NullablePersonId";
            Assert.Equal(expected, joinByLambdaQry.CommandText);
        }

        [Fact]
        public void TotalTest()
        {
            var countFld = SqlField<Person>.Count(p => p.LastName);
            var select = new SqlSelect<Person>()
                .AddFields(p => p.LastName, p => p.Name)
                .AddFields<Passport>(p => p.Number)
                .AddFields(countFld)
                .InnerJoin<Person, Passport>((person, passport) => person.Id == passport.PersonId)
                .Where(SqlFilter<Passport>.From(p => p.Number).IsNotNull().And(p => p.Number).NotEqualTo("3812-808316"))
                .GroupBy(p => p.LastName)
                .Having(SqlFilter<Person>.From<int>(countFld).GreaterThan(2))
                .OrderBy(p => p.LastName);

            var expected =
@"SELECT
    pe.LastName, pe.Name, pa.Number, COUNT(pe.LastName)
FROM
    Person pe
INNER JOIN
    Passport pa ON pe.Id = pa.PersonId
WHERE
    pa.Number IS NOT NULL AND pa.Number <> '3812-808316'
GROUP BY
    pe.LastName
HAVING
    COUNT(pe.LastName) > 2
ORDER BY
    pe.LastName";
            Assert.Equal(expected, select.CommandText);
        }
    }
}

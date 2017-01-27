using System;
using LambdaSqlBuilder.UnitTests.Entities;
using SqlSelectBuilder;
using Xunit;

namespace LambdaSqlBuilder.UnitTests
{
    public class SqlSelectWrapperTest
    {
        [Fact]
        public void SimpleNestedSelect()
        {
            var select = new SqlSelect(new SqlSelect<Person>(), new SqlAlias("p"));
            var expected =
@"SELECT
    *
FROM
(
    SELECT
        *
    FROM
        Person pe
) AS p";
            Assert.Equal(expected, select.CommandText);
        }

        [Fact]
        public void MultiNestedSelect()
        {
            var select = new SqlSelect(new SqlSelect(new SqlSelect<Person>(), new SqlAlias("p0")), new SqlAlias("p1"));
            var expected =
@"SELECT
    *
FROM
(
    SELECT
        *
    FROM
    (
        SELECT
            *
        FROM
            Person pe
    ) AS p0
) AS p1";
            Assert.Equal(expected, select.CommandText);
        }

        [Fact]
        public void IncorrectSelectedFieldsThrowsException()
        {
            var innerQry = new SqlSelect<Person>()
                .AddFields(p => p.LastName);
            var incorrectQry = new SqlSelect(innerQry, new SqlAlias("p"))
                .AddFields<Person>(p => p.Name);
            Assert.Throws<InvalidOperationException>(() => { var cmd = incorrectQry.CommandText; });

            var correctQry = new SqlSelect(innerQry, new SqlAlias("p"))
                .AddFields<Person>(p => p.LastName);
            var expected =
@"SELECT
    p.LastName
FROM
(
    SELECT
        pe.LastName
    FROM
        Person pe
) AS p";
            Assert.Equal(expected, correctQry.CommandText);
        }

        [Fact]
        public void IncorrectSelectedFieldsWithNestedQueryThrowsException()
        {
            var innerQry = new SqlSelect
                (
                new SqlSelect<Person>()
                    .AddFields(p => p.LastName),
                new SqlAlias("p0")
                ).AddFields<Person>(p => p.LastName);
            var incorrectQry = new SqlSelect(innerQry, new SqlAlias("p1"))
                .AddFields<Person>(p => p.Name);
            Assert.Throws<InvalidOperationException>(() => { var cmd = incorrectQry.CommandText; });

            var correctQry = new SqlSelect(innerQry, new SqlAlias("p1"))
                .AddFields<Person>(p => p.LastName);
            var expected =
@"SELECT
    p1.LastName
FROM
(
    SELECT
        p0.LastName
    FROM
    (
        SELECT
            pe.LastName
        FROM
            Person pe
    ) AS p0
) AS p1";
            Assert.Equal(expected, correctQry.CommandText);
        }

        [Fact]
        public void IncorrectFieldAliasThrowsException()
        {
            var innerQry = new SqlSelect<Person>()
                .AddFields(p => p.LastName);

            var alias = new SqlAlias("p");
            var incorrectQry = new SqlSelect(innerQry, alias)
                .AddFields(SqlField<Person>.From(p => p.LastName));
            Assert.Throws<IncorrectAliasException>(() => { var cmd = incorrectQry.CommandText; });
            var correctQry = new SqlSelect(innerQry, alias)
                .AddFields(SqlField<Person>.From(alias, p => p.LastName));
            Assert.DoesNotThrow(() => { var cmd = correctQry.CommandText; });
        }

        [Fact]
        public void IncorrectFilterAliasThrowsException()
        {
            var innerQry = new SqlSelect<Person>()
                .AddFields(p => p.LastName);

            var alias = new SqlAlias("p");
            var whereCheckFail = new SqlSelect(innerQry, alias)
                .Where(SqlFilter<Person>.From(p => p.LastName).EqualTo("Aseev"));
            Assert.Throws<IncorrectAliasException>(() => { var cmd = whereCheckFail.CommandText; });
            var whereCheckPass = new SqlSelect(innerQry, alias)
                .Where(SqlFilter<Person>.From(p => p.LastName, alias).EqualTo("Aseev"));
            Assert.DoesNotThrow(() => { var cmd = whereCheckPass.CommandText; });

            var havingCheckFail = new SqlSelect(innerQry, alias)
                .Having(SqlFilter<Person>.From(p => p.LastName).EqualTo("Aseev"));
            Assert.Throws<IncorrectAliasException>(() => { var cmd = havingCheckFail.CommandText; });
            var havingCheckPass = new SqlSelect(innerQry, alias)
                .Having(SqlFilter<Person>.From(p => p.LastName, alias).EqualTo("Aseev"));
            Assert.DoesNotThrow(() => { var cmd = havingCheckPass.CommandText; });

            var multiCheckFail = new SqlSelect(innerQry, alias)
                .Where(SqlFilter<Person>.From(p => p.LastName).EqualTo("Aseev"))
                .Having(SqlFilter<Person>.From(p => p.LastName).EqualTo("Aseev"));
            Assert.Throws<IncorrectAliasException>(() => { var cmd = multiCheckFail.CommandText; });
            var multiCheckPass = new SqlSelect(innerQry, alias)
                .Where(SqlFilter<Person>.From(p => p.LastName, alias).EqualTo("Aseev"))
                .Having(SqlFilter<Person>.From(p => p.LastName, alias).EqualTo("Aseev"));
            Assert.DoesNotThrow(() => { var cmd = multiCheckPass.CommandText; });
        }
    }
}

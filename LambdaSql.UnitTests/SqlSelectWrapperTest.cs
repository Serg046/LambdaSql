using System;
using LambdaSql.UnitTests.Entities;
using Xunit;

namespace LambdaSql.UnitTests
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
    }
}

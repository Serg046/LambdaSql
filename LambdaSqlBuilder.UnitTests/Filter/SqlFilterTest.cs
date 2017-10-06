﻿using LambdaSqlBuilder.Field;
using LambdaSqlBuilder.Filter;
using LambdaSqlBuilder.UnitTests.Entities;
using Xunit;

namespace LambdaSqlBuilder.UnitTests.Filter
{
    public class SqlFilterTest
    {
        [Fact]
        public void From_LambdaExpression_Success()
        {
            var filter = SqlFilter<Person>.From(m => m.Id).EqualTo(5);

            Assert.Equal("pe.Id = 5", filter.RawSql);
        }

        [Fact]
        public void From_LambdaExpressionWithAlias_Success()
        {
            var filter = SqlFilter<Person>.From(m => m.Id, new SqlAlias<Person>("al")).EqualTo(5);

            Assert.Equal("al.Id = 5", filter.RawSql);
        }

        [Fact]
        public void From_GenericSqlField_Success()
        {
            var filter = SqlFilter<Person>.From(SqlField<Person>.From(m => m.Id)).EqualTo(5);

            Assert.Equal("pe.Id = 5", filter.RawSql);
        }

        [Fact]
        public void From_TypedSqlField_Success()
        {
            var sqlField = new TypedSqlField(typeof(Person), typeof(int))
            {
                Name = nameof(Person.Id),
                Alias = new SqlAlias("pe")
            };
            var filter = SqlFilter<Person>.From(sqlField).EqualTo(5);

            Assert.Equal("pe.Id = 5", filter.RawSql);
        }

        [Fact]
        public void And_LambdaExpression_Success()
        {
            var filter = SqlFilter<Person>.From(m => m.Id).EqualTo(5)
                .And(m => m.Name).EqualTo("Sergey");

            Assert.Equal("pe.Id = 5 AND pe.Name = 'Sergey'", filter.RawSql);
        }

        [Fact]
        public void And_LambdaExpressionWithAlias_Success()
        {
            var filter = SqlFilter<Person>.From(m => m.Id).EqualTo(5)
                .And(m => m.Name, new SqlAlias<Person>("al")).EqualTo("Sergey");

            Assert.Equal("pe.Id = 5 AND al.Name = 'Sergey'", filter.RawSql);
        }

        [Fact]
        public void And_GenericSqlField_Success()
        {
            var filter = SqlFilter<Person>.From(m => m.Id).EqualTo(5)
                .And(SqlField<Person>.From(m => m.Name)).EqualTo("Sergey");

            Assert.Equal("pe.Id = 5 AND pe.Name = 'Sergey'", filter.RawSql);
        }

        [Fact]
        public void And_TypedSqlField_Success()
        {
            var sqlField = new TypedSqlField(typeof(Person), typeof(string))
            {
                Name = nameof(Person.Name),
                Alias = new SqlAlias("pe")
            };
            var filter = SqlFilter<Person>.From(m => m.Id).EqualTo(5)
                .And(sqlField).EqualTo("Sergey");

            Assert.Equal("pe.Id = 5 AND pe.Name = 'Sergey'", filter.RawSql);
        }

        [Fact]
        public void And_SqlFilter_Success()
        {
            var filter = SqlFilter<Person>.From(m => m.Id).EqualTo(5);

            Assert.Equal("pe.Id = 5 AND pe.Id = 5", filter.And(filter).RawSql);
        }

        [Fact]
        public void And_SqlFilterBase_Success()
        {
            var filter = SqlFilter<Person>.From(m => m.Id).EqualTo(5);
            var filter2 = SqlFilter<Passport>.From(m => m.Id).EqualTo(6);

            Assert.Equal("pe.Id = 5 AND pa.Id = 6", filter.And(filter2).RawSql);
        }

        [Fact]
        public void Or_LambdaExpression_Success()
        {
            var filter = SqlFilter<Person>.From(m => m.Id).EqualTo(5)
                .Or(m => m.Name).EqualTo("Sergey");

            Assert.Equal("pe.Id = 5 OR pe.Name = 'Sergey'", filter.RawSql);
        }

        [Fact]
        public void AndGroup_SqlFilter_Success()
        {
            var filter = SqlFilter<Person>.From(m => m.Id).EqualTo(5);

            Assert.Equal("pe.Id = 5 AND (pe.Id = 5)", filter.AndGroup(filter).RawSql);
        }

        [Fact]
        public void AndGroup_SqlFilterBase_Success()
        {
            var filter = SqlFilter<Person>.From(m => m.Id).EqualTo(5);
            var filter2 = SqlFilter<Passport>.From(m => m.Id).EqualTo(6);

            Assert.Equal("pe.Id = 5 AND (pa.Id = 6)", filter.AndGroup(filter2).RawSql);
        }

        [Fact]
        public void Or_LambdaExpressionWithAlias_Success()
        {
            var filter = SqlFilter<Person>.From(m => m.Id).EqualTo(5)
                .Or(m => m.Name, new SqlAlias<Person>("al")).EqualTo("Sergey");

            Assert.Equal("pe.Id = 5 OR al.Name = 'Sergey'", filter.RawSql);
        }

        [Fact]
        public void Or_GenericSqlField_Success()
        {
            var filter = SqlFilter<Person>.From(m => m.Id).EqualTo(5)
                .Or(SqlField<Person>.From(m => m.Name)).EqualTo("Sergey");

            Assert.Equal("pe.Id = 5 OR pe.Name = 'Sergey'", filter.RawSql);
        }

        [Fact]
        public void Or_TypedSqlField_Success()
        {
            var sqlField = new TypedSqlField(typeof(Person), typeof(string))
            {
                Name = nameof(Person.Name),
                Alias = new SqlAlias("pe")
            };
            var filter = SqlFilter<Person>.From(m => m.Id).EqualTo(5)
                .Or(sqlField).EqualTo("Sergey");

            Assert.Equal("pe.Id = 5 OR pe.Name = 'Sergey'", filter.RawSql);
        }

        [Fact]
        public void Or_SqlFilter_Success()
        {
            var filter = SqlFilter<Person>.From(m => m.Id).EqualTo(5);

            Assert.Equal("pe.Id = 5 OR pe.Id = 5", filter.Or(filter).RawSql);
        }

        [Fact]
        public void Or_SqlFilterBase_Success()
        {
            var filter = SqlFilter<Person>.From(m => m.Id).EqualTo(5);
            var filter2 = SqlFilter<Passport>.From(m => m.Id).EqualTo(6);

            Assert.Equal("pe.Id = 5 OR pa.Id = 6", filter.Or(filter2).RawSql);
        }

        [Fact]
        public void OrGroup_SqlFilter_Success()
        {
            var filter = SqlFilter<Person>.From(m => m.Id).EqualTo(5);

            Assert.Equal("pe.Id = 5 OR (pe.Id = 5)", filter.OrGroup(filter).RawSql);
        }

        [Fact]
        public void OrGroup_SqlFilterBase_Success()
        {
            var filter = SqlFilter<Person>.From(m => m.Id).EqualTo(5);
            var filter2 = SqlFilter<Passport>.From(m => m.Id).EqualTo(6);

            Assert.Equal("pe.Id = 5 OR (pa.Id = 6)", filter.OrGroup(filter2).RawSql);
        }

        [Fact]
        public void And_Lambda_InstanceIsImmutable()
        {
            var filter = SqlFilter<Person>.From(m => m.Id).EqualTo(5);

            Assert.Equal("pe.Id = 5 AND pe.Name = 'Sergey'", filter.And(m => m.Name).EqualTo("Sergey").RawSql);
            Assert.Equal("pe.Id = 5", filter.RawSql);
        }

        [Fact]
        public void And_WithoutAliases_Success()
        {
            var filter = SqlFilter<Person>.From(m => m.Id).EqualTo(5)
                .And(m => m.Name).EqualTo("Sergey");

            Assert.Equal("Id = 5 AND Name = 'Sergey'", filter.WithoutAliases().RawSql);
            Assert.Equal("pe.Id = 5 AND pe.Name = 'Sergey'", filter.RawSql);
        }

        [Fact]
        public void And_WithAliases_Success()
        {
            var filter = SqlFilter<Person>.From(m => m.Id).EqualTo(5)
                .And(m => m.Name).EqualTo("Sergey");
            var filterWithoutAliases = filter.WithoutAliases();

            Assert.Equal("Id = 5 AND Name = 'Sergey'", filterWithoutAliases.RawSql);
            Assert.Equal("pe.Id = 5 AND pe.Name = 'Sergey'", filter.RawSql);
            Assert.Equal("pe.Id = 5 AND pe.Name = 'Sergey'", filterWithoutAliases.WithAliases().RawSql);
        }

        [Fact]
        public void And_WithParameterPrefix_Success()
        {
            var filter = SqlFilter<Person>.From(m => m.Id).EqualTo(5);

            Assert.Equal("pe.Id = @p0", filter.ParametricSql);
            Assert.Equal("pe.Id = @prm0", filter.WithParameterPrefix("prm").ParametricSql);
        }
    }
}

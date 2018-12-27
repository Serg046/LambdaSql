using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Xunit;

namespace LambdaSql.UnitTests
{
    public class MetadataProviderTest
    {
        // ReSharper disable ClassNeverInstantiated.Local
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        private class TestEntity
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class TestEntity2 : TestEntity
        {
        }

        private class TestEntity3 : TestEntity
        {
        }
        // ReSharper restore ClassNeverInstantiated.Local

        [Fact]
        public void TableNameIsValid()
        {
            Assert.Equal("TestEntity", MetadataProvider.Instance.GetTableName<TestEntity>());
            Assert.Equal("TestEntity2", MetadataProvider.Instance.GetTableName<TestEntity2>());
            Assert.Equal("TestEntity3", MetadataProvider.Instance.GetTableName<TestEntity3>());
        }

        [Fact]
        public void PropertyNameIsValid()
        {
            Expression<Func<TestEntity, object>> test1 = entity => entity.Name;
            Expression<Func<TestEntity2, object>> test2 = entity => entity.Id;
            Expression<Func<TestEntity3, object>> test3 = entity => entity.Id;

            Assert.Equal("Name", MetadataProvider.Instance.GetPropertyName(test1));
            Assert.Equal("Id", MetadataProvider.Instance.GetPropertyName(test2));
            Assert.Equal("Id", MetadataProvider.Instance.GetPropertyName(test3));
        }

        [Theory]
        [InlineData("5", "'5'")]
        [InlineData(true, "1")]
        [InlineData(false, "0")]
        [InlineData(5, "5")]
        public void ParameterToString_SomeValueWithoutDbType_ConvertedCorrectly(object value, string expectedString)
        {
            Assert.Equal(expectedString, MetadataProvider.Instance.ParameterToString(value));
        }

        [Theory]
        [InlineData("5", DbType.Int32, "5")]
        [InlineData(5, DbType.String, "'5'")]
        [InlineData(true, DbType.Boolean, "1")]
        [InlineData(false, DbType.Boolean, "0")]
        public void ParameterToString_SomeValueWithDbType_ConvertedCorrectly(object value, DbType dbType, string expectedString)
        {
            Assert.Equal(expectedString, MetadataProvider.Instance.ParameterToString(value, dbType));
        }

        [Fact]
        public void ThrowExceptionsForSimilarEntities()
        {
            MetadataProvider.Instance.AliasFor<TestEntity>();
            Assert.Throws<DuplicateAliasException>(() => MetadataProvider.Instance.AliasFor<TestEntity2>());
            Assert.Throws<DuplicateAliasException>(() => MetadataProvider.Instance.AliasFor<TestEntity3>());
            MetadataProvider.Instance.AliasFor<TestEntity>();
        }
    }
}

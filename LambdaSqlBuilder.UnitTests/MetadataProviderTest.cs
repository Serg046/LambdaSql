using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Xunit;

namespace LambdaSqlBuilder.UnitTests
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

        [Fact]
        public void ParameterIsConvertedCorrectly()
        {
            Assert.Equal("5", MetadataProvider.Instance.ParameterToString(5));
            Assert.Equal("'5'", MetadataProvider.Instance.ParameterToString("5"));
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

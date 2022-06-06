using CommunAxiom.Commons.Ingestion.Configuration;
using CommunAxiom.Commons.Ingestion.DataSource;
using NUnit.Framework;
using FluentAssertions;
using System;
using Moq;
using Microsoft.Extensions.DependencyInjection;

namespace CommunAxiom.Commons.Ingestion.Tests.DataSource
{
    [TestFixture]
    public class DataSourceFactoryTest
    {
        private MockRepository _mockRepository;
        private Mock<IServiceProvider> _serviceProvider;

        private DataSourceFactory _sourceFactory;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _serviceProvider = _mockRepository.Create<IServiceProvider>();

            SetupMock();

            _sourceFactory = new DataSourceFactory(_serviceProvider.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _mockRepository.Verify();
        }

        [Test]
        public void WhenDataSourceTypeIsFileThenSourceFactoryShouldCreateTextDataSourceReader()
        {
            _serviceProvider.Setup(x => x.GetService(typeof(TextDataSourceReader))).Returns(new TextDataSourceReader());

            var reader = _sourceFactory.Create(DataSourceType.File);
            reader.Should().BeOfType<TextDataSourceReader>();
        }


        [Test]
        public void WhenDataSourceTypeIsNotFoundThenSourceFactoryShouldThrowArgumentException()
        {
            var sourceType = DataSourceType.API;
            _serviceProvider.Setup(x => x.GetService(typeof(TextDataSourceReader))).Returns(new TextDataSourceReader());

            Action act = () => _sourceFactory.Create(sourceType);

            act.Should()
                .Throw<ArgumentException>()
                .WithMessage($"No DataSourceReader type with name {Enum.GetName(sourceType)} could be found");
        }

        [Test]
        public void WhenServiceProviderIsResolvedNullThenSourceFactoryShouldThrowNullReferenceException()
        {
            var sourceType = DataSourceType.File;

            _serviceProvider.Setup(x => x.GetService(typeof(TextDataSourceReader))).Returns(null);

            Action act = () => _sourceFactory.Create(sourceType);

            act.Should()
                .Throw<NullReferenceException>()
                .WithMessage($"No DataSourceReader resolved with type {typeof(TextDataSourceReader).FullName}");
        }

        private void SetupMock()
        {

            var serviceScope = new Mock<IServiceScope>();
            serviceScope.Setup(x => x.ServiceProvider).Returns(_serviceProvider.Object);

            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            serviceScopeFactory.Setup(x => x.CreateScope()).Returns(serviceScope.Object);

            _serviceProvider.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactory.Object);
        }
    }
}


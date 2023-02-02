using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;
using CommunAxiom.Commons.Client.Contracts.Ingestion.Validators;
using CommunAxiom.Commons.Ingestion.DataSource;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;

namespace CommunAxiom.Commons.Ingestion.Tests.DataSource
{
    [TestFixture]
    public class DataSourceFactoryTest
    {
        private MockRepository _mockRepository;
        private Mock<Func<DataSourceType, IDataSourceReader>> _serviceResolver;
        private Mock<IConfigValidatorLookup> _configValidatorLookup;

        private DataSourceFactory _sourceFactory;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _serviceResolver = _mockRepository.Create<Func<DataSourceType, IDataSourceReader>>();
            _configValidatorLookup = _mockRepository.Create<IConfigValidatorLookup>();

            // SetupMock();

            _sourceFactory = new DataSourceFactory(_serviceResolver.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _mockRepository.Verify();
        }

        [Test]
        public void WhenDataSourceTypeIsFileThenSourceFactoryShouldCreateTextDataSourceReader()
        {
            var textDataSourceReader = new TextDataSourceReader(_configValidatorLookup.Object);

            _serviceResolver.Setup(x => x.Invoke(DataSourceType.File)).Returns(textDataSourceReader);

            var reader = _sourceFactory.Create(DataSourceType.File);
            reader.Should().BeOfType<TextDataSourceReader>();
        }

        [Test]
        public void WhenServiceProviderIsResolvedNullThenSourceFactoryShouldThrowNullReferenceException()
        {
            var sourceType = DataSourceType.File;

            _serviceResolver.Setup(x => x.Invoke(DataSourceType.File)).Returns(() => null);

            Action act = () => _sourceFactory.Create(sourceType);

            act.Should().Throw<NullReferenceException>().WithMessage("No DataSourceReader resolved!");
        }
    }
}


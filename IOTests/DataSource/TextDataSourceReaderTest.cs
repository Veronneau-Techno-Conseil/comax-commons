using CommunAxiom.Commons.Ingestion.Configuration;
using CommunAxiom.Commons.Ingestion.DataSource;
using CommunAxiom.Commons.Ingestion.Validators;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CommunAxiom.Commons.Ingestion.Tests.DataSource
{
    [TestFixture]
    public class TextDataSourceReaderTest
    {
        private readonly TextDataSourceReader _textDataSourceReader;
        private readonly Mock<IConfigValidatorLookup> _configValidatorLookup;
        private readonly MockRepository _mockRepository;

        public TextDataSourceReaderTest()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _configValidatorLookup = _mockRepository.Create<IConfigValidatorLookup>();
            _textDataSourceReader = new TextDataSourceReader(_configValidatorLookup.Object);
        }

        [Test]
        public void WhenTextDataSourceReaderThenIngestionTypeShouldBeJSON()
        {
            _textDataSourceReader.IngestorType.Should().Be(IngestorType.JSON);
        }

    }
}


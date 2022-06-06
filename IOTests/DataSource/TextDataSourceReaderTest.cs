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
        private readonly Mock<IFieldValidatorLookup> _fieldValidatorLookup;
        private readonly MockRepository _mockRepository;

        public TextDataSourceReaderTest()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _fieldValidatorLookup = _mockRepository.Create<IFieldValidatorLookup>();
            _textDataSourceReader = new TextDataSourceReader();
        }

        [SetUp]
        public void Setup()
        {
            var requiredFieldValidatorurn = new RequiredFieldValidator();
            _fieldValidatorLookup.Setup(x => x.Get("required")).Returns(requiredFieldValidatorurn);
        }

        [TearDown]
        public void TearDown()
        {
            _mockRepository.Verify();
        }

        [Test]
        public void WhenTextDataSourceReaderThenIngestionTypeShouldBeJSON()
        {
            _textDataSourceReader.IngestionType.Should().Be(IngestionType.JSON);
        }

    }
}


using System;
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
        private readonly TextDataSourceReader textDataSourceReader;
        private readonly Mock<IFieldValidatorLookup> mock;
        private readonly MockRepository mockRepository;

        public TextDataSourceReaderTest()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mock = mockRepository.Create<IFieldValidatorLookup>();

            textDataSourceReader = new TextDataSourceReader();
        }

        [SetUp]
        public void Setup()
        {
            mock.Verify();
            var requiredFieldValidatorurn = new RequiredFieldValidator();
            mock.Setup(x => x.Get("required")).Returns(requiredFieldValidatorurn);
        }

        [TearDown]
        public void TearDown()
        {
            mock.Verify();
        }

        [Test]
        public void WhenTextDataSourceReaderThenIngestionTypeShouldBeJSON()
        {
            textDataSourceReader.IngestionType.Should().Be(IngestionType.JSON);
        }

    }
}


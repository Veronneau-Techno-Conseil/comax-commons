using CommunAxiom.Commons.Client.Contracts.Ingestion.Validators;
using CommunAxiom.Commons.Ingestion.Attributes;
using CommunAxiom.Commons.Ingestion.Ingestor;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;

namespace CommunAxiom.Commons.Ingestion.Tests.Ingestor
{
    [TestFixture]
    public class JsonIngestorTest
    {
        private readonly MockRepository _mockRepository;
        private readonly Mock<IFieldValidatorLookup> _fieldValidatorLookup;
       
        private readonly JsonIngestor _jsonIngestor;

        public JsonIngestorTest()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _fieldValidatorLookup = _mockRepository.Create<IFieldValidatorLookup>();
            _jsonIngestor = new JsonIngestor(_fieldValidatorLookup.Object);
        }

        [Test]
        public void JsonIngestorShouldHasIngestionTypeAttribute()
        {
            var attr = Attribute.GetCustomAttribute(typeof(JsonIngestor), typeof(IngestionTypeAttribute));
            
            attr.Should().NotBeNull();
        }
    }
}

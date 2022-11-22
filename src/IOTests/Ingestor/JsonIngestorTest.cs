using CommunAxiom.Commons.Ingestion.Attributes;
using CommunAxiom.Commons.Ingestion.Ingestor;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace CommunAxiom.Commons.Ingestion.Tests.Ingestor
{
    [TestFixture]
    public class JsonIngestorTest
    {
        private readonly JsonIngestor _jsonIngestor;

        public JsonIngestorTest()
        {
            _jsonIngestor = new JsonIngestor();
        }

        [Test]
        public void JsonIngestorShouldHasIngestionTypeAttribute()
        {
            var attr = Attribute.GetCustomAttribute(typeof(JsonIngestor), typeof(IngestionTypeAttribute));
            
            attr.Should().NotBeNull();
        }
    }
}

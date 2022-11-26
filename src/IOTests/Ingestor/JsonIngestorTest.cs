using CommunAxiom.Commons.Ingestion.Ingestor;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;
using CommunAxiom.Commons.Client.Contracts.Ingestion.Validators;
using CommunAxiom.Commons.Ingestion.Validators;

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
        public async Task WhenParseAsyncThenShouldReturnRows()
        {
            await using var stream = new FileStream("Samples/Files/sample.json", FileMode.Open, FileAccess.Read);
           
            _jsonIngestor.Configure(new List<FieldMetaData>
            {
                new()
                {
                    FieldName = "property1",
                    Validators = new List<IFieldValidator>
                    {
                        new RequiredFieldValidator()
                    }
                }
            });
            
            var result = await _jsonIngestor.ParseAsync(stream);

            result.Errors.Count.Should().Be(0);
            result.Rows.Count.Should().Be(2);
        }
        
        [Test]
        public async Task WhenParseAsyncHasErrorThenShouldReturnErrors()
        {
            await using var stream = new FileStream("Samples/Files/sample.json", FileMode.Open, FileAccess.Read);
           
            _jsonIngestor.Configure(new List<FieldMetaData>
            {
                new()
                {
                    FieldName = "property3",
                    Validators = new List<IFieldValidator>
                    {
                        new RequiredFieldValidator()
                    }
                }
            });
            
            var result = await _jsonIngestor.ParseAsync(stream);

            result.Errors[0].Item2.ErrorCode.Should().Be("This field is required!");
        }
    }
}

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;
using CommunAxiom.Commons.Client.Contracts.Ingestion.Validators;
using CommunAxiom.Commons.Ingestion.Ingestor;
using CommunAxiom.Commons.Ingestion.Validators;
using FluentAssertions;
using NUnit.Framework;

namespace CommunAxiom.Commons.Ingestion.Tests.Ingestor
{
    [TestFixture]
    public class CsvIngestorTest
    {
        private readonly CsvIngestor _csvIngestor;

        public CsvIngestorTest()
        {
            _csvIngestor = new CsvIngestor();
        }
    
        [Test]
        public async Task WhenParseAsyncThenShouldReturnRows()
        {
            await using var stream = new FileStream("Samples/Files/sample.csv", FileMode.Open, FileAccess.Read);
           
            _csvIngestor.Configure(new List<FieldMetaData>
            {
                new()
                {
                    FieldName = "Organization Id",
                    Validators = new List<IFieldValidator>
                    {
                        new RequiredFieldValidator()
                    }
                }
            });
            
            var result = await _csvIngestor.ParseAsync(stream);

            result.Errors.Count.Should().Be(0);
            result.Rows.Count.Should().Be(100);
        }
        
        
        [Test]
        public async Task WhenParseAsyncHasErrorThenShouldReturnErrors()
        {
            await using var stream = new FileStream("Samples/Files/sample.csv", FileMode.Open, FileAccess.Read);
           
            _csvIngestor.Configure(new List<FieldMetaData>
            {
                new()
                {
                    FieldName = "Organization",
                    Validators = new List<IFieldValidator>
                    {
                        new RequiredFieldValidator()
                    }
                }
            });
            
            var result = await _csvIngestor.ParseAsync(stream);

            result.Errors[0].Item2.ErrorCode.Should().Be("This field is required!");
            result.Rows.Count.Should().Be(0);
        }
        
    }
}
using CommunAxiom.Commons.Ingestion.Configuration;
using CommunAxiom.Commons.Ingestion.DataSource;
using CommunAxiom.Commons.Ingestion.Ingestor;
using Moq;
using NUnit.Framework;
using FluentAssertions;
using CommunAxiom.Commons.Ingestion.Validators;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CommunAxiom.Commons.Ingestion.Tests
{
    [TestFixture]
    public class ImporterTest
    {
        private readonly MockRepository _mockRepository;
        private readonly Mock<IDataSourceFactory> _dataSourceFactory;
        private readonly Mock<IIngestorFactory> _ingestorFactory;
        private readonly Mock<IConfigValidatorLookup> _configValidatorLookup;
        private readonly Mock<IFieldValidatorLookup> _fieldValidatorLookup;

        private readonly Importer _importer;

        public ImporterTest()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _dataSourceFactory = new Mock<IDataSourceFactory>();
            _ingestorFactory = new Mock<IIngestorFactory>();
            _configValidatorLookup = new Mock<IConfigValidatorLookup>();
            _fieldValidatorLookup = new Mock<IFieldValidatorLookup>();

            _importer = new Importer(_dataSourceFactory.Object, _ingestorFactory.Object);
        }

        [Test]
        public async Task WhenImporterReturnResults()
        {
            var file = new File { Name = "sample1.txt", Path = "Samples/Files" };

            var sourceConfig = new SourceConfig
            {
                DataSourceType = DataSourceType.File,
                Configurations = new Dictionary<string, DataSourceConfiguration>
                {
                    {
                        "sample datasource",
                        new DataSourceConfiguration
                        {
                            FieldType = FieldType.File,
                            Value = JsonConvert.SerializeObject(file)
                        }
                    }
                }
            };

            _ingestorFactory.Setup(x => x.Create(IngestorType.JSON)).Returns(new JsonIngestor(_fieldValidatorLookup.Object));
            _dataSourceFactory.Setup(x => x.Create(sourceConfig.DataSourceType)).Returns(new TextDataSourceReader(_configValidatorLookup.Object));

            var result = await _importer.Import(sourceConfig);

            result.Errors.Should().BeNull();
            result.results.Should().NotBeEmpty();
        }

    }
}


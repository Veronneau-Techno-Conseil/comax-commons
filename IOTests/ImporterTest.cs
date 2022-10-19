using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;
using CommunAxiom.Commons.Client.Contracts.Ingestion.Validators;
using CommunAxiom.Commons.Ingestion.DataSource;
using CommunAxiom.Commons.Ingestion.Ingestor;
using CommunAxiom.Commons.Ingestion.Validators;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Ingestion.Tests
{
    [TestFixture]
    public class ImporterTest
    {
        private MockRepository _mockRepository;
        private Mock<IDataSourceFactory> _dataSourceFactory;
        private Mock<IIngestorFactory> _ingestorFactory;
        private Mock<IConfigValidatorLookup> _configValidatorLookup;
        private Mock<IFieldValidatorLookup> _fieldValidatorLookup;

        private Importer _importer;


        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _dataSourceFactory = new Mock<IDataSourceFactory>();
            _ingestorFactory = new Mock<IIngestorFactory>();
            _configValidatorLookup = new Mock<IConfigValidatorLookup>();
            _fieldValidatorLookup = new Mock<IFieldValidatorLookup>();

            _importer = new Importer(_dataSourceFactory.Object, _ingestorFactory.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _mockRepository.Verify();
        }

        [Test]
        public async Task WhenImporterReturnRows()
        {
            var sourceConfig = new SourceConfig
            {
                DataSourceType = DataSourceType.FILE,
                Configurations = new Dictionary<string, DataSourceConfiguration>
                {
                    {
                        "SampleFile",
                        new DataSourceConfiguration
                        {
                            Name = "SampleFile",
                            FieldType = ConfigurationFieldType.File,
                            Value = JsonConvert.SerializeObject(new FileModel { Name = "sample1.txt", Path = "Samples/Files" })
                        }
                    }
                }
            };

            var field = new FieldMetaData
            {
                DisplayName = "sample1",
                FieldName = "property1",
                Validators = new List<IFieldValidator>() { new RequiredFieldValidator() }
            };

            var fields = new List<FieldMetaData> { field };

            _ingestorFactory.Setup(x => x.Create(IngestorType.JSON)).Returns(new JsonIngestor(_fieldValidatorLookup.Object));
            _dataSourceFactory.Setup(x => x.Create(sourceConfig.DataSourceType)).Returns(new TextDataSourceReader(_configValidatorLookup.Object));
            _fieldValidatorLookup.Setup(x => x.Get("required-field")).Returns(new RequiredFieldValidator());

            var result = await _importer.Import(sourceConfig, fields);

            result.Errors.Should().BeEmpty();
            result.Rows.Count.Should().Be(2);
        }

        [Test]
        public async Task WhenImporterReturnErrors()
        {
            var sourceConfig = new SourceConfig
            {
                DataSourceType = DataSourceType.FILE,
                Configurations = new Dictionary<string, DataSourceConfiguration>
                {
                    {
                        "SampleFile",
                        new DataSourceConfiguration
                        {
                            Name = "SampleFile",
                            FieldType = ConfigurationFieldType.File,
                            Value = JsonConvert.SerializeObject(new FileModel { Name = "sample2.txt", Path = "Samples/Files" })
                        }
                    }
                }
            };

            var field = new FieldMetaData
            {
                DisplayName = "sample1",
                FieldName = "property",
                Validators = new List<IFieldValidator> { new RequiredFieldValidator() }
            };

            var fields = new List<FieldMetaData> { field };

            _ingestorFactory.Setup(x => x.Create(IngestorType.JSON)).Returns(new JsonIngestor(_fieldValidatorLookup.Object));
            _dataSourceFactory.Setup(x => x.Create(sourceConfig.DataSourceType)).Returns(new TextDataSourceReader(_configValidatorLookup.Object));
            _fieldValidatorLookup.Setup(x => x.Get("required-field")).Returns(new RequiredFieldValidator());

            var result = await _importer.Import(sourceConfig, fields);

            result.Rows.Should().BeEmpty();
            result.Errors.Count.Should().Be(2);
        }
    }
}


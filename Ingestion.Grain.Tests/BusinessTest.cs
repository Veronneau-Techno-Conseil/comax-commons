using CommunAxiom.Commons.Client.Contracts.Datasource;
using CommunAxiom.Commons.Client.Contracts.Grains.Storage;
using CommunAxiom.Commons.Client.Contracts.IO;
using CommunAxiom.Commons.Client.Grains.IngestionGrain;
using CommunAxiom.Commons.Ingestion;
using CommunAxiom.Commons.Ingestion.Configuration;
using CommunAxiom.Commons.Ingestion.DataSource;
using CommunAxiom.Commons.Ingestion.Ingestor;
using CommunAxiom.Commons.Ingestion.Validators;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using FluentAssertions;
using Newtonsoft.Json.Linq;

namespace Ingestion.Grain.Tests
{
    [TestFixture]
    public class BusinessTest
    {
        private Business _business;
        private MockRepository _mockRepository;
        private Mock<IDataSourceFactory> _dataSourceFactory;
        private Mock<IIngestorFactory> _ingestorFactory;
        private Mock<IGrainFactory> _grainFactory;
        private Mock<IConfigValidatorLookup> _configValidatorLookup;
        private Mock<IFieldValidatorLookup> _fieldValidatorLookup;
        private Mock<IDatasource> _dataSource;
        private const string grainKey = "grain-key-test";

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _dataSourceFactory = new Mock<IDataSourceFactory>();
            _ingestorFactory = new Mock<IIngestorFactory>();
            _grainFactory = new Mock<IGrainFactory>();
            _configValidatorLookup = new Mock<IConfigValidatorLookup>();
            _fieldValidatorLookup = new Mock<IFieldValidatorLookup>();
            _dataSource = new Mock<IDatasource>();

            var importer = new Importer(_dataSourceFactory.Object, _ingestorFactory.Object);
            _business = new Business(importer, _grainFactory.Object, grainKey);
        }

        [TearDown]
        public void TearDown()
        {
            _mockRepository.Verify();
        }

        [Test]
        public async Task RunWhenNoErrors()
        {
            _dataSourceFactory.Setup(o => o.Create(DataSourceType.File)).Returns(new TextDataSourceReader(_configValidatorLookup.Object));
            _ingestorFactory.Setup(x => x.Create(IngestorType.JSON)).Returns(new JsonIngestor(_fieldValidatorLookup.Object));

            var field = new FieldMetaData
            {
                DisplayName = "sample1",
                FieldName = "property1",
                Validators = new List<IFieldValidator>() { new RequiredFieldValidator() }
            };

            var sourceState = new SourceState
            {
                Configurations = new Dictionary<string, DataSourceConfiguration>
                {
                    {
                        "file-type",
                        new DataSourceConfiguration
                        {
                            Name = "file1",
                            FieldType = FieldType.File,
                            Value = JsonConvert.SerializeObject(new FileModel { Name = "sample1.txt", Path = "Samples/Files" })
                        }
                    }
                },
                DataSourceType = DataSourceType.File,
                Fields = new List<FieldMetaData> { field }
            };

            _fieldValidatorLookup.Setup(x => x.Get("required-field")).Returns(new RequiredFieldValidator());

            _dataSource.Setup(o => o.GetState()).ReturnsAsync(sourceState);

            _grainFactory.Setup(o => o.GetGrain<IDatasource>(grainKey, null)).Returns(_dataSource.Object);

            var mockStorageGrain = new MockStorageGrain();

            _grainFactory.Setup(o => o.GetGrain<IStorageGrain>(It.IsAny<string>(), null)).Returns(mockStorageGrain);


            await _business.Run();

            var data = await mockStorageGrain.GetData();

            data.Count.Should().Be(3);

            var row1 = JObject.Parse(@"{'property1': 'sample property 1', 'property2': 'sample property 2'}");
            var row2 = JObject.Parse(@"{ 'property1': 'sample property 1 - 1', 'property4': 'sample property 4'}");
            var row3 = JObject.Parse(@"{ 'indexes': ['grain-key-test-0','grain-key-test-1'] }");
            
            data.Should().BeEquivalentTo(new List<JObject> { row1, row2, row3 });

        }

    }
}
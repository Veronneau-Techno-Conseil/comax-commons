using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;
using CommunAxiom.Commons.Client.Contracts.Ingestion.Validators;
using CommunAxiom.Commons.Ingestion.Attributes;
using CommunAxiom.Commons.Ingestion.DataSource;
using CommunAxiom.Commons.Ingestion.Validators;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace CommunAxiom.Commons.Ingestion.Tests.DataSource
{
    [TestFixture]
    public class TextDataSourceReaderTest
    {
        private TextDataSourceReader _textDataSourceReader;
        private Mock<IConfigValidatorLookup> _configValidatorLookup;
        private MockRepository _mockRepository;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _configValidatorLookup = _mockRepository.Create<IConfigValidatorLookup>();
            _textDataSourceReader = new TextDataSourceReader(_configValidatorLookup.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _mockRepository.Verify();
        }

        [Test]
        public void TextDataSourceReaderShouldHasIngestionTypeAttribute()
        {
            var attr = Attribute.GetCustomAttribute(typeof(TextDataSourceReader), typeof(DataSourceTypeAttribute));

            attr.Should().NotBeNull();
        }

        [Test]
        public void WhenTextDataSourceReaderThenIngestionTypeShouldBeJson()
        {
            _configValidatorLookup
                .Setup(x => x.Get(It.IsAny<ConfigurationFieldType>()))
                .Returns(new List<IConfigValidator>());
            
            _configValidatorLookup
                .Setup(x => x.Get(It.IsAny<ConfigurationFieldType>(), It.IsAny<string>()))
                .Returns(new List<IConfigValidator>());
            
            
            _textDataSourceReader.Setup();
            _textDataSourceReader.IngestorType.Should().Be(IngestorType.JSON);
        }

        [Test]
        public void ShouldReadDataReturnsStream()
        {
            _configValidatorLookup
                .Setup(x => x.Get(It.IsAny<ConfigurationFieldType>()))
                .Returns(new List<IConfigValidator>());
            
            _configValidatorLookup
                .Setup(x => x.Get(It.IsAny<ConfigurationFieldType>(), It.IsAny<string>()))
                .Returns(new List<IConfigValidator>());
            
            
            _textDataSourceReader.Setup(new SourceConfig
            {
                DataSourceType = DataSourceType.File,
                Configurations = new Dictionary<string, DataSourceConfiguration>
                {
                    {
                        "SampleFile",
                        new DataSourceConfiguration
                        {
                            Name = "SampleFile",
                            FieldType = ConfigurationFieldType.File,
                            Value = "Samples/files/sample1.txt"
                        }
                    },
                    {
                        "FilePath",
                        new DataSourceConfiguration
                        {
                            Name = "FilePath",
                            FieldType = ConfigurationFieldType.File,
                            Value = "Samples/Files/sample1.txt"
                        }
                    }
                }
            });
            
            using var stream = _textDataSourceReader.ReadData();
            
            var actual = ReadAsStream(stream);

            var expected = ReadFile("Samples/files/sample1.txt");
            
            actual.Should().Be(expected);
        }

        [Test]
        public void WhenConfigurationIsNotSetupThenReadDataShouldThrowNullReferenceException()
        {
            Action act = () => _textDataSourceReader.ReadData();
            act.Should()
                .Throw<NullReferenceException>()
                .WithMessage("There is no data source configuration!");
        }


        [Test]
        public void WhenDataSourceConfigurationSetFileTypeWithNoFileValueThenValidateConfigurationShouldReturnValidationError()
        {
            var sourceConfig = new SourceConfig
            {
                DataSourceType = DataSourceType.File,
                Configurations = new Dictionary<string, DataSourceConfiguration>
                {
                    {
                        "SampleFile",
                        new DataSourceConfiguration { Name = "SampleFile", Value = null, FieldType = ConfigurationFieldType.File }
                    }
                }
            };

            var validators = new Dictionary<ConfigurationFieldType, IList<IConfigValidator>>
            {
                {
                    ConfigurationFieldType.File,
                    new List<IConfigValidator> { new FileConfigValidator() }
                }
            };
            
            _configValidatorLookup
                .Setup(x => x.Get(It.IsAny<ConfigurationFieldType>()))
                .Returns(new List<IConfigValidator>());
            
            _configValidatorLookup
                .Setup(x => x.Get(It.IsAny<ConfigurationFieldType>(), It.IsAny<string>()))
                .Returns(new List<IConfigValidator>());
            
            _textDataSourceReader.Setup(sourceConfig);

            foreach (var actual in _textDataSourceReader.ValidateConfiguration())
            {
                if (actual == null) continue;

                actual.ErrorCode.Should().Be("File is not exists or file length is zero.");
                actual.FieldName.Should().Be("SampleFile");
            }
        }

        private string ReadAsStream(Stream stream)
        {
            using var sr = new StreamReader(stream);
            return sr.ReadToEnd();
        }

        private string ReadFile(string filePath)
        {
            using var expectedStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return ReadAsStream(expectedStream);
        }
    }
}
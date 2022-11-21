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
        public void WhenTextDataSourceReaderThenIngestionTypeShouldBeJSON()
        {
            _textDataSourceReader.Setup();
            _textDataSourceReader.IngestorType.Should().Be(IngestorType.JSON);
        }

        [Test]
        public void ShouldReadDataReturnsStream()
        {
            var file = new FileModel { Name = "sample1.txt", Path = "Samples/Files" };
            _textDataSourceReader.Setup(new SourceConfig
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
            var actual = ReadAsString(stream);

            var expected = ReadAsString(file);

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
                DataSourceType = DataSourceType.FILE,
                Configurations = new Dictionary<string, DataSourceConfiguration>
                {
                    { "SampleFile", new DataSourceConfiguration { Name = "SampleFile", FieldType = ConfigurationFieldType.File }}
                }
            };

            var validators = new List<IConfigValidator> { new FileConfigValidator() };
            _configValidatorLookup.Setup(x => x.ConfigValidators).Returns(validators);

            _textDataSourceReader.Setup(sourceConfig);

            foreach (var actual in _textDataSourceReader.ValidateConfiguration())
            {
                if (actual == null) continue;

                actual.ErrorCode.Should().Be("The file type is required to set file name and file path");
                actual.FieldName.Should().Be("SampleFile");
            }
        }

        private string ReadAsString(Stream stream)
        {
            using var sr = new StreamReader(stream);
            return sr.ReadToEnd();
        }

        private string ReadAsString(FileModel file)
        {
            using var expectedStream = new FileStream(Path.Combine(file.Path, file.Name), FileMode.Open, FileAccess.Read);
            return ReadAsString(expectedStream);
        }
    }
}


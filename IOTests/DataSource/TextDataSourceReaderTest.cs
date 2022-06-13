using CommunAxiom.Commons.Ingestion.Configuration;
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
        private readonly TextDataSourceReader _textDataSourceReader;
        private readonly Mock<IConfigValidatorLookup> _configValidatorLookup;
        private readonly MockRepository _mockRepository;

        public TextDataSourceReaderTest()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _configValidatorLookup = _mockRepository.Create<IConfigValidatorLookup>();
            _textDataSourceReader = new TextDataSourceReader(_configValidatorLookup.Object);
        }

        [Test]
        public void WhenTextDataSourceReaderThenIngestionTypeShouldBeJSON()
        {
            _textDataSourceReader.IngestorType.Should().Be(IngestorType.JSON);
        }

        [Test]
        public void ShouldReadDataReturnsStream()
        {
            var file = new Configuration.File { Name = "sample1.txt", Path = "Samples/Files" };
            _textDataSourceReader.Setup(new SourceConfig
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
            });

            using var stream = _textDataSourceReader.ReadData();
            var actual = ReadAsString(stream);

            var expected = ReadAsString(file);

            actual.Should().Be(expected);
        }

        [Test]
        public void WhenConfigurationIsNotSetupThenReadDataShouldThrowNullReferenceException()
        {
            _textDataSourceReader.ClearCofigurations();

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
                    { "config1", new DataSourceConfiguration { Name = "file1", FieldType = FieldType.File }}
                }
            };

            var validators = new List<IConfigValidator> { new FileConfigValidator() };
            _configValidatorLookup.Setup(x => x.ConfigValidators).Returns(validators);

            _textDataSourceReader.Setup(sourceConfig);

            foreach (var actual in _textDataSourceReader.ValidateConfiguration())
            {
                actual.ErrorCode.Should().Be("The file type is required to set file name and file path");
                actual.FieldName.Should().Be("file1");
            }
        }

        private string ReadAsString(Stream stream)
        {
            using var sr = new StreamReader(stream);
            return sr.ReadToEnd();
        }

        private string ReadAsString(Configuration.File file)
        {
            using var expectedStream = new FileStream(Path.Combine(file.Path, file.Name), FileMode.Open, FileAccess.Read);
            return ReadAsString(expectedStream);
        }
    }
}


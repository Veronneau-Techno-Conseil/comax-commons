using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;
using CommunAxiom.Commons.Ingestion.Validators;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace CommunAxiom.Commons.Ingestion.Tests.Validators
{
    [TestFixture]
    public class FileConfigValidatorTest
    {
        private readonly FileConfigValidator _fileConfigValidator;

        public FileConfigValidatorTest()
        {
            _fileConfigValidator = new FileConfigValidator();
        }

        [Test]
        public void WhenDataSourceConfigurationIsNotSetFileTypeThenShouldReturnNull()
        {
            var dataSourceConfiguration = new DataSourceConfiguration
            {
                Name = "file1",
                FieldType = ConfigurationFieldType.Boolean
            };

            var actual = _fileConfigValidator.Validate(dataSourceConfiguration);

            actual.Should().BeNull();
        }

        [Test]
        public void WhenDataSourceConfigurationIsSetFileTypeWithNoValueThenShouldReturnValidationError()
        {
            var dataSourceConfiguration = new DataSourceConfiguration
            {
                Name = "file1",
                FieldType = ConfigurationFieldType.File
            };

            var actual = _fileConfigValidator.Validate(dataSourceConfiguration);

            actual.FieldName.Should().Be("file1");
            actual.ErrorCode.Should().Be("File is not exists or file length is zero.");
        }

        [Test]
        public void WhenDataSourceConfigurationIsSetFileTypeWithValue()
        {
            var file = new FileModel { Name = "name", Path = "path" };
            var dataSourceConfiguration = new DataSourceConfiguration
            {
                Name = "file1",
                FieldType = ConfigurationFieldType.File,
                Value = JsonConvert.SerializeObject(file)
            };

            var actual = _fileConfigValidator.Validate(dataSourceConfiguration);

            actual.Should().BeNull();
        }
    }
}

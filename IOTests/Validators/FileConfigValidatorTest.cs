using CommunAxiom.Commons.Ingestion.Configuration;
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
            var dataSourceConfiguration = new DataSourceConfiguration { Name = "file1", FieldType = FieldType.Object };

            var actual = _fileConfigValidator.Validate(dataSourceConfiguration);

            actual.Should().BeNull();
        }

        [Test]
        public void WhenDataSourceConfigurationIsSetFileTypeWithNoValueThenShouldReturnValidationError()
        {
            var dataSourceConfiguration = new DataSourceConfiguration { Name = "file1", FieldType = FieldType.File };

            var actual = _fileConfigValidator.Validate(dataSourceConfiguration);

            actual.FieldName.Should().Be("file1");
            actual.ErrorCode.Should().Be("The file type is required to set file name and file path");
        }

        [Test]
        public void WhenDataSourceConfigurationIsSetFileTypeWithValue()
        {
            var file = new File { Name = "name", Path = "path" };
            var dataSourceConfiguration = new DataSourceConfiguration { Name = "file1", FieldType = FieldType.File, Value = JsonConvert.SerializeObject(file) };

            var actual = _fileConfigValidator.Validate(dataSourceConfiguration);

            actual.Should().BeNull();
        }
    }
}

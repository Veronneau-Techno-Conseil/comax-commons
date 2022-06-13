using CommunAxiom.Commons.Ingestion.Configuration;
using CommunAxiom.Commons.Ingestion.Validators;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace CommunAxiom.Commons.Ingestion.Tests.Validators
{
    [TestFixture]
    public class RequiredFieldValidatorTest
    {
        private RequiredFieldValidator _requiredFieldValidator;

        public RequiredFieldValidatorTest()
        {
            _requiredFieldValidator = new RequiredFieldValidator();
        }

        [Test]
        public void TagNameSholdBeRequiredField()
        {
            _requiredFieldValidator.Tag.Should().Be("required-field");
        }

        [Test]
        public void WhenDataSourceConfigurationIsNullThenShouldReturnNull()
        {
            var obj = new JObject();
            obj.Add("file1", "some value");

            var actual = _requiredFieldValidator.Validate(null, obj);

            actual.Should().BeNull();
        }

        [Test]
        public void WhenJObjectIsNullThenShouldReturnNull()
        {
            var dataSourceConfiguration = new DataSourceConfiguration { FieldType = FieldType.Text, Name = "file1" };
            var actual = _requiredFieldValidator.Validate(dataSourceConfiguration, null);

            actual.Should().BeNull();
        }

        [Test]
        public void WhenFieldIsRequired()
        {
            var dataSourceConfiguration = new DataSourceConfiguration { FieldType = FieldType.Text, Name = "file1" };

            var obj = new JObject();
            obj.Add("file1", "some value");

            var actual = _requiredFieldValidator.Validate(dataSourceConfiguration, obj);
            
            actual.Should().BeNull();
        }

        [Test]
        public void WhenFieldIsRequiredButNotInJObjectThenShouldReturnValidationError()
        {
            var dataSourceConfiguration = new DataSourceConfiguration { FieldType = FieldType.Text, Name = "file1" };

            var obj = new JObject();
            obj.Add("file2", "some value");

            var actual = _requiredFieldValidator.Validate(dataSourceConfiguration, obj);

            actual.FieldName.Should().Be("file1");
            actual.ErrorCode.Should().Be("This field is required!");
        }
    }
}

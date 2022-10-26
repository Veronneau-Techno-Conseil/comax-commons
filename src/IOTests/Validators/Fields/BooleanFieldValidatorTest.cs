using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;
using CommunAxiom.Commons.Ingestion.Validators;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace CommunAxiom.Commons.Ingestion.Tests.Validators
{
    [TestFixture]
    public class BooleanFieldValidatorTest
    {
        private BooleanFieldValidator _booleanFieldValidator;

        public BooleanFieldValidatorTest()
        {
            _booleanFieldValidator = new BooleanFieldValidator();
        }

        [Test]
        public void TagNameSholdBeBooleanFieldType()
        {
            _booleanFieldValidator.Tag.Should().Be("boolean-field-type");
        }

        [Test]
        public void WhenFieldIsBooleanThenShouldReturnNull()
        {
            var field = new FieldMetaData { FieldName = "p1", FieldType = FieldType.Boolean };
            JObject row = new JObject();
            row.Add("p1", true);

            var result = _booleanFieldValidator.Validate(field, row);
            result.Should().BeNull();
        }


        [Test]
        public void WhenFieldIsNotBooleanThenShouldReturnValidationErrorCode()
        {
            var field = new FieldMetaData { FieldName = "p1", FieldType = FieldType.Boolean };
            JObject row = new JObject();
            row.Add("p1", "true");

            var result = _booleanFieldValidator.Validate(field, row);
            result.ErrorCode.Should().Be("This field has been indicated to be number!");
        }


    }
}

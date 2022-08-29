using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;
using CommunAxiom.Commons.Ingestion.Validators;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace CommunAxiom.Commons.Ingestion.Tests.Validators
{
    [TestFixture]
    public class NumberFieldValidatorTest
    {
        private NumberFieldValidator _numberFieldValidator;

        public NumberFieldValidatorTest()
        {
            _numberFieldValidator = new NumberFieldValidator();
        }

        [Test]
        public void TagNameSholdBeNumberFieldType()
        {
            _numberFieldValidator.Tag.Should().Be("number-field-type");
        }

        [Test]
        public void WhenFieldIsNumberThenShouldReturnNull()
        {
            var field = new FieldMetaData { FieldName = "p1", FieldType = FieldType.Number };
            JObject row = new JObject();
            row.Add("p1", 1);

            var result = _numberFieldValidator.Validate(field, row);
            result.Should().BeNull();
        }


        [Test]
        public void WhenFieldIsNotNumberThenShouldReturnValidationErrorCode()
        {
            var field = new FieldMetaData { FieldName = "p1", FieldType = FieldType.Number };
            JObject row = new JObject();
            row.Add("p1", "1");

            var result = _numberFieldValidator.Validate(field, row);
            result.ErrorCode.Should().Be("This field has been indicated to be number!");
        }


    }
}

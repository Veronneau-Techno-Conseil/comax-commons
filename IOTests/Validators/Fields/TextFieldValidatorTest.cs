using CommunAxiom.Commons.Ingestion.Configuration;
using CommunAxiom.Commons.Ingestion.Validators;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace CommunAxiom.Commons.Ingestion.Tests.Validators
{
    [TestFixture]
    public class TextFieldValidatorTest
    {
        private TextFieldValidator _textFieldValidator;

        public TextFieldValidatorTest()
        {
            _textFieldValidator = new TextFieldValidator();
        }

        [Test]
        public void TagNameSholdBeTextFieldType()
        {
            _textFieldValidator.Tag.Should().Be("text-field-type");
        }

        [Test]
        public void WhenFieldIsTextThenShouldReturnNull()
        {
            var field = new FieldMetaData { FieldName = "p1", FieldType = FieldType.Text };
            JObject row = new JObject();
            row.Add("p1", "sample text");

            var result = _textFieldValidator.Validate(field, row);
            result.Should().BeNull();
        }


        [Test]
        public void WhenFieldIsNotTextThenShouldReturnValidationErrorCode()
        {
            var field = new FieldMetaData { FieldName = "p1", FieldType = FieldType.Text };
            JObject row = new JObject();
            row.Add("p1", 1);

            var result = _textFieldValidator.Validate(field, row);
            result.ErrorCode.Should().Be("This field has been indicated to be number!");
        }


    }
}

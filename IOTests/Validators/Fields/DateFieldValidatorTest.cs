using CommunAxiom.Commons.Ingestion.Configuration;
using CommunAxiom.Commons.Ingestion.Validators;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;

namespace CommunAxiom.Commons.Ingestion.Tests.Validators
{
    [TestFixture]
    public class DateFieldValidatorTest
    {
        private DateFieldValidator _dateFieldValidator;

        public DateFieldValidatorTest()
        {
            _dateFieldValidator = new DateFieldValidator();
        }

        [Test]
        public void TagNameSholdBeDateFieldType()
        {
            _dateFieldValidator.Tag.Should().Be("date-field-type");
        }

        [Test]
        public void WhenFieldIsDateThenShouldReturnNull()
        {
            var field = new FieldMetaData { FieldName = "p1", FieldType = FieldType.Date };
            JObject row = new JObject();
            row.Add("p1", new DateTime());

            var result = _dateFieldValidator.Validate(field, row);
            result.Should().BeNull();
        }


        [Test]
        public void WhenFieldIsNotDateThenShouldReturnValidationErrorCode()
        {
            var field = new FieldMetaData { FieldName = "p1", FieldType = FieldType.Date };
            JObject row = new JObject();
            row.Add("p1", "2020-01-01");

            var result = _dateFieldValidator.Validate(field, row);
            result.ErrorCode.Should().Be("This field has been indicated to be number!");
        }


    }
}

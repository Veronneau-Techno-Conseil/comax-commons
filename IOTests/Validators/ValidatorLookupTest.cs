using NUnit.Framework;
using FluentAssertions;
using CommunAxiom.Commons.Ingestion.Validators;

namespace CommunAxiom.Commons.Ingestion.Tests.Validators
{
    [TestFixture]
    public class ValidatorLookupTest
    {
        private  ValidatorLookup _validatorLookup;

        [SetUp]
        public void SetUp()
        {
            _validatorLookup = new ValidatorLookup();
        }

        [Test]
        public void GetFieldValidator()
        {
            _validatorLookup.Add(new RequiredFieldValidator());
            var result = _validatorLookup.Get("required-field");

            result.Should().BeOfType<RequiredFieldValidator>();
        }

        [Test]
        public void TryGetFieldValidator()
        {
            _validatorLookup.Add(new RequiredFieldValidator());

            IFieldValidator validator;

            var result = _validatorLookup.TryGet("required-field", out validator);

            result.Should().BeTrue();
            validator.Should().BeOfType<RequiredFieldValidator>();
        }


        [Test]
        public void GetConfigValidator()
        {
            _validatorLookup.Add(new FileConfigValidator());
            var result = _validatorLookup.ConfigValidators;

            result[0].Should().BeOfType<FileConfigValidator>();
        }
    }
}

using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;
using CommunAxiom.Commons.Ingestion.Validators;
using FluentAssertions;
using NUnit.Framework;

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
            _validatorLookup.Add(FieldType.Boolean, new BooleanFieldValidator());

            var result = _validatorLookup.Get(FieldType.Boolean);

            result[0].Should().BeOfType<BooleanFieldValidator>();
        }
        
        [Test]
        public void GetConfigValidator()
        {
            _validatorLookup.Add(ConfigurationFieldType.File, new FileConfigValidator());
            
            var result = _validatorLookup.Get(ConfigurationFieldType.File);

            result[0].Should().BeOfType<FileConfigValidator>();
        }
    }
}

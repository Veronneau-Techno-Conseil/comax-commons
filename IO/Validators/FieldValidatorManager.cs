using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.IO.Configuration.Validators
{
    public class FieldValidatorManager : IFieldValidator
    {
        private readonly IEnumerable<IFieldValidator> _validators;
        private FieldValidatorManager(IEnumerable<IFieldValidator> fieldValidators)
        {
            _validators = fieldValidators.ToArray();
        }

        public static IFieldValidator Package(params IFieldValidator[] fieldValidators)
        {
            return new FieldValidatorManager(fieldValidators);
        }

        public void Configure(FieldMetadata field)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ValidationError> ValidateConfig(FieldMetadata field)
        {
            foreach (var validator in _validators)
            {
                var res = validator.ValidateConfig(field);
                if (res != null && res.Count() > 0)
                {
                    foreach (ValidationError error in res)
                        yield return error;
                }
            }
        }

        public IEnumerable<ValidationError> ValidateData(FieldMetadata field, JObject o)
        {
            foreach (var validator in _validators)
            {
                var res = validator.ValidateData(field, o);
                if (res != null && res.Count() > 0)
                {
                    foreach (ValidationError error in res)
                        yield return error;
                }
            }
        }
    }
}

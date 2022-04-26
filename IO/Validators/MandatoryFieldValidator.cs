using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.IO.Configuration.Validators
{
    public class MandatoryFieldValidator : IFieldValidator
    {
        private static IFieldValidator _instance;
        public static IFieldValidator Instance
        {
            get { return _instance ??= new MandatoryFieldValidator(); }
        }

        private MandatoryFieldValidator()
        {

        }

        public IEnumerable<ValidationError> ValidateData(FieldMetadata field, JObject o)
        {
            if(field == null)
                yield break;
            if(field.Mandatory && (o[field.Name] == null || !o[field.Name].HasValues))
            {
                yield return new ValidationError { FieldName = field.Name, ErrorCode = ValidationError.MANDATORY };
            }
            yield break;
        }

        public IEnumerable<ValidationError> ValidateConfig(FieldMetadata field)
        {
            yield break;
        }
    }
}

using CommunAxiom.Commons.Client.IO.Configuration.Validators;
using Newtonsoft.Json.Linq;


namespace CommunAxiom.Commons.Client.IO.Configuration.MetadataFactory.ConcreteFactories
{
    public class TextFileFieldManager : IFieldManager
    {
        private TextFileFieldManager()
        {
            
        }

        public static TextFileFieldManager Instance { get; } = new TextFileFieldManager();

        public void Configure(FieldMetadata field)
        {
            field.FieldType = FieldType.TextFile;
            field.Manager = this;
        }

        public IEnumerable<ValidationError> ValidateConfig(FieldMetadata field)
        {
            List<IFieldValidator> fieldValidators = new List<IFieldValidator>();

            if (field.Mandatory)
                fieldValidators.Add(MandatoryFieldValidator.Instance);
            fieldValidators.Add(new Validator());

            IFieldValidator composite = FieldValidatorManager.Package(
                fieldValidators.ToArray()
            );
            return composite.ValidateConfig(field);
        }

        public IEnumerable<ValidationError> ValidateData(FieldMetadata field, JObject o)
        {
            yield break;
        }

        public class Contract
        {
            public string Path { get; set; }
        }

        protected class Validator : IFieldValidator
        {
            public IEnumerable<ValidationError> ValidateConfig(FieldMetadata field)
            {
                var param = Newtonsoft.Json.JsonConvert.DeserializeObject<Contract>(field.Parameter);
                if (param == null || string.IsNullOrWhiteSpace(param.Path))
                {
                    yield return new ValidationError
                    {
                        ErrorCode = ValidationError.MANDATORY,
                        FieldName = "Parameter"
                    };
                }
                yield break;
            }

            public IEnumerable<ValidationError> ValidateData(FieldMetadata field, JObject o)
            {
                yield break;
            }
        }
    }
}

using CommunAxiom.Commons.Client.IO.Configuration.MetadataFactory.ConcreteFactories;
using CommunAxiom.Commons.Client.IO.Configuration.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.IO.Configuration.ValidatorFactory
{
    public static class Factory
    {
        public static FieldMetadata Create(FieldType fieldType, Action<FieldMetadata> configure)
        {
            FieldMetadata field = new FieldMetadata();

            switch (fieldType)
            {
                case FieldType.TextFile:
                    TextFileFieldManager.Instance.Configure(field);
                    configure(field);
                    return field;
            }

            throw new InvalidOperationException("Field type unknown");
        }

        public static FieldMetadata Rehydrate(FieldMetadata field, Action<FieldMetadata> configure)
        {
            switch (field.FieldType)
            {
                case FieldType.TextFile:
                    TextFileFieldManager.Instance.Configure(field);
                    configure(field);
                    return field;
            }

            throw new InvalidOperationException("Field type unknown");
        }
    }
}

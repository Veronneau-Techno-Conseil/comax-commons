using System;
using CommunAxiom.Commons.Ingestion.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace CommunAxiom.Commons.Ingestion.Extentions
{
    public static class IngestionBuilderExtentions
    {
        public static void SetupIngestion(this IServiceCollection services)
        {
            services.AddTransient<IFieldValidatorLookup, FieldValidatorOptions>();

            var fieldValidatorManager = new FieldValidatorManager();
            fieldValidatorManager.Configure(options =>
            {
                 options.Add(new RequiredFieldValidator());
            });
        }

    }
}


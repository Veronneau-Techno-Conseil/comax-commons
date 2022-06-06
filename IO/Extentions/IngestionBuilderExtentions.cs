using CommunAxiom.Commons.Ingestion.DataSource;
using CommunAxiom.Commons.Ingestion.Ingestor;
using CommunAxiom.Commons.Ingestion.Injestor;
using CommunAxiom.Commons.Ingestion.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace CommunAxiom.Commons.Ingestion.Extentions
{
    public static class IngestionBuilderExtentions
    {
        public static void AddIngestion(this IServiceCollection services)
        {
            services.AddTransient<IFieldValidatorLookup, FieldValidatorOptions>();

            // data sources
            services.AddTransient<IDataSourceReader, TextDataSourceReader>();

            // ingestors
            services.AddTransient<IIngestor, JsonIngestor>();

            // factories
            services.AddTransient<IDataSourceFactory, DataSourceFactory>();
            services.AddTransient<IIngestionFactory, IngestorFactory>();

            // validations
            var fieldValidatorManager = new FieldValidatorManager();
            fieldValidatorManager.Configure(options =>
            {
                options.Add(new RequiredFieldValidator());
            });
        }

    }
}


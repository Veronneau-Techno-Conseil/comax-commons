using CommunAxiom.Commons.Ingestion.DataSource;
using CommunAxiom.Commons.Ingestion.Ingestor;
using CommunAxiom.Commons.Ingestion.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace CommunAxiom.Commons.Ingestion.Extentions
{
    public static class IngestionBuilderExtentions
    {
        public static void AddIngestion(this IServiceCollection services)
        {
            services.AddTransient<IFieldValidatorLookup, ValidatorOptions>();
            services.AddTransient<IConfigValidatorLookup, ValidatorOptions>();

            // data sources
            services.AddTransient<IDataSourceReader, TextDataSourceReader>();

            // ingestors
            services.AddTransient<IIngestor, JsonIngestor>();

            // factories
            services.AddTransient<IDataSourceFactory, DataSourceFactory>();
            services.AddTransient<IIngestorFactory, IngestorFactory>();

            // validations
            var fieldValidatorManager = new ValidatorManager();
            fieldValidatorManager.Configure(options =>
            {
                options.Add(new RequiredFieldValidator());
            });

            fieldValidatorManager.Configure(options =>
            {
                options.Add(new FileConfigValidator());
            });
        }

    }
}


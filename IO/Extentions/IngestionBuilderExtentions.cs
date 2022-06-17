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
            services.AddTransient<IFieldValidatorLookup, ValidatorLookup>();
            services.AddTransient<IConfigValidatorLookup, ValidatorLookup>();

            // data sources
            services.AddTransient<IDataSourceReader, TextDataSourceReader>();

            // ingestors
            services.AddTransient<IIngestor, JsonIngestor>();

            // factories
            services.AddTransient<IDataSourceFactory, DataSourceFactory>();
            services.AddTransient<IIngestorFactory, IngestorFactory>();

            // validations
            var fieldValidatorManager = new ValidatorManager();
            fieldValidatorManager.ConfigureFields(options =>
            {
                options.Add(new RequiredFieldValidator());
                options.Add(new NumberFieldValidator());
                options.Add(new BooleanFieldValidator());
                options.Add(new DateFieldValidator());
                options.Add(new TextFieldValidator());
            });

            fieldValidatorManager.ConfigureConfigs(options =>
            {
                options.Add(new FileConfigValidator());
            });
        }

    }
}


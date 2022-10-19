using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;
using CommunAxiom.Commons.Client.Contracts.Ingestion.Validators;
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

            services.AddScoped<TextDataSourceReader>();
            
            services.AddTransient<Func<DataSourceType, IDataSourceReader>>(provider => key =>
            {
                switch (key)
                {
                    case DataSourceType.FILE:
                        return provider.GetService<TextDataSourceReader>();
                    case DataSourceType.API:
                        return null;
                }

                return null;
            });

            // ingestors
            services.AddTransient<IIngestor, JsonIngestor>();

            // factories
            services.AddTransient<IDataSourceFactory, DataSourceFactory>();
            services.AddTransient<IIngestorFactory, IngestorFactory>();
            services.AddTransient<IMetadataParser, JSONMetadataParser>();
            
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


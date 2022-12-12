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
            var validatorLookup = new ValidatorLookup();

            services.AddSingleton<IFieldValidatorLookup>(validatorLookup);
            services.AddSingleton<IConfigValidatorLookup>(validatorLookup);

            // data sources

            services.AddScoped<TextDataSourceReader>();
            
            services.AddTransient<Func<DataSourceType, IDataSourceReader>>(provider => key =>
            {
                switch (key)
                {
                    case DataSourceType.File:
                        return provider.GetService<TextDataSourceReader>();
                }

                return null;
            });

            // ingestors
            services.AddScoped<JsonIngestor>();
            services.AddScoped<CsvIngestor>();
            
            services.AddTransient<Func<IngestorType, IIngestor>>(provider => key =>
            {
                switch (key)
                {
                    case IngestorType.JSON:
                        return provider.GetService<JsonIngestor>();
                    case IngestorType.CSV:
                        return provider.GetService<CsvIngestor>();
                }

                return null;
            });

            // factories
            services.AddTransient<IDataSourceFactory, DataSourceFactory>();
            services.AddTransient<IIngestorFactory, IngestorFactory>();
            services.AddTransient<IMetadataParser, JSONMetadataParser>();
            
            // validations
            var fieldValidatorManager = new ValidatorManager(validatorLookup);
            fieldValidatorManager.ConfigureFields(options =>
            {
                options.Add( "required", new RequiredFieldValidator());
                options.Add(FieldType.Decimal, new NumberFieldValidator());
                options.Add(FieldType.Integer, new NumberFieldValidator());
                options.Add(FieldType.Boolean, new BooleanFieldValidator());
                options.Add(FieldType.Date,new DateFieldValidator());
                options.Add(FieldType.Text, new TextFieldValidator());
            });

            fieldValidatorManager.ConfigureConfigs(options =>
            {
                options.Add(ConfigurationFieldType.File, new FileConfigValidator());
                
                options.Add("required", new RequiredConfigValidator());
                // options.Add(ConfigurationFieldType.Date, new RequireConfigValidator());
            });
            
            services.AddSingleton<Importer, Importer>();
        }

    }
}


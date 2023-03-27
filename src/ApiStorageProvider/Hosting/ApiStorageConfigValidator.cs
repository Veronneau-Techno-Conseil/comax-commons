using Orleans;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.StorageProvider.Hosting
{
    public class ApiStorageConfigValidator : IConfigurationValidator
    {
        private readonly ApiStorageConfiguration _config;
        private readonly string _name;
        public ApiStorageConfigValidator(string name, ApiStorageConfiguration dbConfig)
        {
            _config = dbConfig;
            _name = name;
        }

        public void ValidateConfiguration()
        {
            if(_config == null || string.IsNullOrWhiteSpace(_config.ApiStorageUri))
            {
                throw new InvalidOperationException($"ApiStorageConfiguration configuration invalid");
            }
        }
    }
}

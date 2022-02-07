using Orleans;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.StorageProvider.Hosting
{
    public class LiteDbConfigValidator : IConfigurationValidator
    {
        private readonly LiteDbConfig _config;
        private readonly string _name;
        public LiteDbConfigValidator(string name, LiteDbConfig dbConfig)
        {
            _config = dbConfig;
            _name = name;
        }

        public void ValidateConfiguration()
        {
            if(_config == null || string.IsNullOrWhiteSpace(_config.FileName))
            {
                throw new InvalidOperationException($"LiteDbStorage configuration invalid");
            }
        }
    }
}

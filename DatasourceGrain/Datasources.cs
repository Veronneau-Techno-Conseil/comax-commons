using CommunAxiom.Commons.Client.Contracts.Datasource;
using CommunAxiom.Commons.Client.Contracts.IO;
using Newtonsoft.Json.Linq;
using Orleans;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Grains.DatasourceGrain
{
    public class Datasources : Grain, IDatasource
    {
        private readonly Business _business;
        public Datasources([PersistentState("DataSource")]IPersistentState<SourceState> state)
        {
            _business = new Business(new Repo(state));
        }

        public Task<SourceState> GetConfig()
        {
            return _business.ReadConfig();
        }

        public async Task SetConfig(SourceState sourceState)
        {
            await _business.WriteConfig(sourceState);
        }

        public async Task DeleteConfig()
        {
            await _business.DeleteConfig();
        }

    }
}

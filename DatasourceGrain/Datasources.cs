using CommunAxiom.Commons.Client.Contracts.Datasource;
using CommunAxiom.Commons.Client.Contracts.IO;
using Orleans;
using System;
using System.Threading.Tasks;

namespace DatasourceGrain
{
    public class Datasources : Grain, IDatasource
    {
        public Task<SourceState> GetState()
        {
            throw new NotImplementedException();
        }

        public Task<string> TestGrain(string Grain)
        {
            return Task.FromResult($"The {Grain} grain has been launched. Check it on the dashboard");
        }
    }
}

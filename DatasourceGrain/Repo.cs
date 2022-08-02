using CommunAxiom.Commons.Client.Contracts.IO;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Grains.DatasourceGrain
{
    public class Repo
    {
        private readonly IPersistentState<SourceState> _dataSourceState;

        public Repo(IPersistentState<SourceState> dataSourceState)
        {
            _dataSourceState = dataSourceState;
        }

        public async Task<SourceState> ReadConfig()
        {
            await _dataSourceState.ReadStateAsync();
            return _dataSourceState.State;
        }

        public async Task WriteConfig(SourceState state)
        {
            _dataSourceState.State = state;
            await _dataSourceState.WriteStateAsync();
        }

        public async Task DeleteConfig()
        {
            await _dataSourceState.ClearStateAsync();
        }
    }
}

using CommunAxiom.Commons.Client.Contracts.IO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Grains.DatasourceGrain
{
    public class Business
    {
        private readonly Repo _repo;

        public Business(Repo repo)
        {
            _repo = repo;
        }

        public async Task WriteConfig(SourceState state)
        {
            //todo should be validate all configurations here? 
            await _repo.WriteConfig(state);
        }

        public async Task<SourceState> ReadConfig()
        {
            return await _repo.ReadConfig();
        }

        public Task DeleteConfig()
        {
            return _repo.DeleteConfig();
        }
    }
}

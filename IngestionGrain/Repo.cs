using CommunAxiom.Commons.Client.Contracts.IO;
using Orleans.Runtime;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Grains.IngestionGrain
{
    public class Repo
    {
        private readonly IPersistentState<SourceState>  _sourceItem;
        public Repo(IPersistentState<SourceState> sourceItem)
        {
            _sourceItem = sourceItem;
        }

        //public async Task<SourceState> Fetch()
        //{
        //    await _sourceItem.ReadStateAsync();
        //    return _sourceItem.State;
        //}

        public async Task AddHistory(SourceState accountDetails)
        {
            _sourceItem.State = accountDetails;
            await _sourceItem.WriteStateAsync();
        }
    }
}

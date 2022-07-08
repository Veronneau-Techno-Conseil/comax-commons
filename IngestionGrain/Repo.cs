using Orleans.Runtime;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Grains.IngestionGrain
{
    public class Repo
    {
        private readonly IPersistentState<History>  _sourceItem;
        public Repo(IPersistentState<History> sourceItem)
        {
            _sourceItem = sourceItem;
        }

        //public async Task<SourceState> Fetch()
        //{
        //    await _sourceItem.ReadStateAsync();
        //    return _sourceItem.State;
        //}

        public async Task AddHistory(History accountDetails)
        {
            _sourceItem.State = accountDetails;
            await _sourceItem.WriteStateAsync();
        }
    }
}

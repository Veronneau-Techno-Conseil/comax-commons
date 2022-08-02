using CommunAxiom.Commons.Client.Contracts.Ingestion;
using Orleans.Runtime;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Grains.IngestionGrain
{
    public class Repo
    {
        private readonly IPersistentState<IngestionHistory>  _history;
        public Repo(IPersistentState<IngestionHistory> history)
        {
            _history = history;
        }

        public async Task<IngestionHistory> FetchHistory()
        {
            await _history.ReadStateAsync();
            return _history.State;
        }

        public async Task AddHistory(IngestionHistory newHistory)
        {
            _history.State = newHistory;
            await _history.WriteStateAsync();
        }
    }
}

using Comax.Commons.Orchestrator.Contracts.Portfolio.Models;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.SharedPortfolio.Registry
{
    public class PortfolioRegistryDal
    {
        private bool _read = false;
        private readonly IPersistentState<PortfolioRegistryState> _registry;
        public PortfolioRegistryDal(IPersistentState<PortfolioRegistryState> registry) 
        { 
            _registry= registry;
        }

        internal async Task EnsureRead()
        {
            if (!_read)
            {
                await _registry.ReadStateAsync();
                if (_registry.State.PortfolioOwners == null)
                    _registry.State.PortfolioOwners = new List<PortfolioInfo>();
                _read = true;
            }
        }
        
        public async Task<List<PortfolioInfo>> List()
        {
            await EnsureRead();
            return _registry.State.PortfolioOwners.ToList();
        }

        public async Task Upsert(PortfolioInfo info)
        {
            await EnsureRead();
            if(!_registry.State.PortfolioOwners.Any(x=>x.PortfolioUri == info.PortfolioUri))
            {
                _registry.State.PortfolioOwners.Add(info);
                await _registry.WriteStateAsync();
            }
        }

        public async Task Remove(string uri)
        {
            await EnsureRead();
            _registry.State.PortfolioOwners.RemoveAll(x => x.PortfolioUri.ToLower() == uri.ToLower());    
            await _registry.WriteStateAsync();            
        }
    }
}

using Comax.Commons.Orchestrator.Contracts.ComaxSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.AgentService.OrchClient
{
    public class ClientManager
    {
        public Task InitializeTask { get; }
        private readonly IOrchestratorClientFactory _orchestratorClientFactory;
        public bool IsConnectionActive { get; private set; }
        public IOrchestratorClient? Client { get; private set; }

        public ClientManager(IOrchestratorClientFactory orchestratorClientFactory) 
        {
            _orchestratorClientFactory= orchestratorClientFactory;
            InitializeTask = this.Initialize();
        }

        public async Task Initialize()
        {
            IsConnectionActive = await _orchestratorClientFactory.TestConnection();
            Client = await _orchestratorClientFactory.GetUnmanagedClient();
        }
    }
}

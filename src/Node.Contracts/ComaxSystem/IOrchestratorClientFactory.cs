using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.Contracts.ComaxSystem
{
    public interface IOrchestratorClientFactory
    {
        Task<bool> TestConnection();
        Task WithClusterClient(Func<IOrchestratorClient, Task> action);
        Task<TResult> WithClusterClient<TResult>(Func<IOrchestratorClient, Task<TResult>> action);
        Task<IOrchestratorClient> GetUnmanagedClient();
        Task<IOrchestratorClient> WithUnmanagedClient(Func<IOrchestratorClient, Task> action);
        Task<(IOrchestratorClient, TResult)> WithUnmanagedClient<TResult>(Func<IOrchestratorClient, Task<TResult>> action);
    }
}

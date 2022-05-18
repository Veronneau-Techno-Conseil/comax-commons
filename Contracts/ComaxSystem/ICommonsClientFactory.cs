using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Contracts.ComaxSystem
{
    public interface ICommonsClientFactory
    {
        Task<bool> TestConnection();
        Task WithClusterClient(Func<ICommonsClusterClient, Task> action);
        Task<TResult> WithClusterClient<TResult>(Func<ICommonsClusterClient, Task<TResult>> action);
        Task<ICommonsClusterClient> WithUnmanagedClient(Func<ICommonsClusterClient, Task> action);
        Task<(ICommonsClusterClient, TResult)> WithUnmanagedClient<TResult>(Func<ICommonsClusterClient, Task<TResult>> action);
    }
}

using CommunAxiom.Commons.Client.Contracts.Account;
using CommunAxiom.Commons.Client.Contracts.Auth;
using CommunAxiom.Commons.Client.Contracts.ComaxSystem;
using CommunAxiom.Commons.Client.Contracts.Datasource;
using CommunAxiom.Commons.Client.Contracts.DataTransfer;
using CommunAxiom.Commons.Client.Contracts.Grains.Portfolio;
using CommunAxiom.Commons.Client.Contracts.Ingestion;
using CommunAxiom.Commons.Client.Contracts.Project;
using CommunAxiom.Commons.Client.Contracts.Replication;
using CommunAxiom.Commons.Shared;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Contracts.ComaxSystem
{
    public interface ICommonsClusterClient: IDisposable
    {
        IAccount GetAccount();
        IAuthentication GetAuthentication();
        IDatasource GetDatasource(string datasourceId);
        IDataTransfer GetDataTransfer(string operationId);
        IIngestion GetIngestion(string ingestionId);
        IPortfolio GetPortfolio(string portfolioId);
        IProject GetProject(string projectId);
        IReplication GetReplication();
        Task Close();
        Task<StreamSubscriptionHandle<SystemEvent>> SubscribeSystem(Func<SystemEvent, StreamSequenceToken, Task> fn, Func<Exception, Task> funcError, Func<Task> onCompleted);
        Task<(StreamSubscriptionHandle<AuthorizationInstructions>, AsyncEnumerableStream<AuthorizationInstructions>)> SubscribeAuth(Guid streamId);
        IStreamProvider GetStreamProvider(string name);
    }
}

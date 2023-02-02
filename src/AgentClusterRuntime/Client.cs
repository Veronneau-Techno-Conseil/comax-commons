using CommunAxiom.Commons.Client.Contracts;
using CommunAxiom.Commons.Client.Contracts.Account;
using CommunAxiom.Commons.Client.Contracts.Auth;
using CommunAxiom.Commons.Client.Contracts.ComaxSystem;
using CommunAxiom.Commons.Client.Contracts.Datasource;
using CommunAxiom.Commons.Client.Contracts.DataTransfer;
using CommunAxiom.Commons.Client.Contracts.Grains.Portfolio;
using CommunAxiom.Commons.Client.Contracts.Grains.Scheduler;
using CommunAxiom.Commons.Client.Contracts.Ingestion;
using CommunAxiom.Commons.Client.Contracts.Project;
using CommunAxiom.Commons.Client.Contracts.Replication;
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Shared;
using Orleans;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunAxiom.Commons.Client.Contracts.Grains.DateStateMonitorSupervisor;
using CommunAxiom.Commons.CommonsShared.Contracts.EventMailbox;
using CommunAxiom.Commons.CommonsShared.Contracts.UriRegistry;
using Microsoft.Extensions.Logging;
using CommunAxiom.Commons.Client.Contracts.Grains.Agent;

namespace CommunAxiom.Commons.Client.AgentClusterRuntime
{
    public static class Client 
    {
        public static IDataStateMonitorSupervisor GetDateStateMonitorSupervisor(this IComaxGrainFactory grainFactory)
        {
            return grainFactory.GetGrain<IDataStateMonitorSupervisor>(Guid.Empty);
        }

        public static IAccount GetAccount(this IComaxGrainFactory grainFactory)
        {
            return grainFactory.GetGrain<IAccount>(Guid.Empty);
        }

        public static IAgent GetAgent(this IComaxGrainFactory grainFactory)
        {
            return grainFactory.GetGrain<IAgent>(Guid.Empty);
        }

        public static IAuthentication GetAuthentication(this IComaxGrainFactory grainFactory)
        {
            return grainFactory.GetGrain<IAuthentication>(Guid.Empty);
        }

        public static IDatasource GetDatasource(this IComaxGrainFactory grainFactory, string datasourceId)
        {
            return grainFactory.GetGrain<IDatasource>(datasourceId);
        }

        public static IDataTransfer GetDataTransfer(this IComaxGrainFactory grainFactory, string operationId)
        {
            return grainFactory.GetGrain<IDataTransfer>(operationId);
        }

        public static IIngestion GetIngestion(this IComaxGrainFactory grainFactory, string ingestionId)
        {
            return grainFactory.GetGrain<IIngestion>(ingestionId);
        }

        public static IPortfolio GetPortfolio(this IComaxGrainFactory grainFactory)
        {
            return grainFactory.GetGrain<IPortfolio>(Guid.Empty);
        }

        public static IProject GetProject(this IComaxGrainFactory grainFactory, string projectId)
        {
            return grainFactory.GetGrain<IProject>(projectId);
        }

        public static IReplication GetReplication(this IComaxGrainFactory grainFactory)
        {
            return grainFactory.GetGrain<IReplication>(Guid.Empty);
        }

        public static IScheduler GetScheduler(this IComaxGrainFactory grainFactory, int schedulerID)
        {
            return grainFactory.GetGrain<IScheduler>(schedulerID);
        }

        public static async Task<StreamSubscriptionHandle<SystemEvent>> SubscribeSystem(this IComaxGrainFactory grainFactory, Func<SystemEvent, StreamSequenceToken, Task> fn, Func<Exception, Task> funcError, Func<Task> onCompleted)
        {
            var res = await grainFactory.GetStreamProvider(OrleansConstants.Streams.DefaultStream)
                    .GetStream<SystemEvent>(Guid.Empty, OrleansConstants.StreamNamespaces.DefaultNamespace)
                    .SubscribeAsync(fn, funcError, onCompleted);
            return res;
        }

        public static async Task<(StreamSubscriptionHandle<AuthorizationInstructions>, AsyncEnumerableStream<AuthorizationInstructions>)> SubscribeAuth(this IComaxGrainFactory grainFactory, Guid streamId)
        {
            var result = new AsyncEnumerableStream<AuthorizationInstructions>();
            var handle = await grainFactory.GetStreamProvider(OrleansConstants.Streams.DefaultStream)
                    .GetStream<AuthorizationInstructions>(streamId, OrleansConstants.StreamNamespaces.DefaultNamespace)
                    .SubscribeAsync(result);
            return (handle, result);
        }

        public static IUriRegistry GetUriRegistry(this IComaxGrainFactory grainFactory, string uri)
        {
            string actual = OrleansConstants.BLANK_ID;
            if (!string.IsNullOrWhiteSpace(uri))
            {
                if (!uri.StartsWith("usr://") && !uri.StartsWith("com://"))
                {
                    throw new ArgumentException("Id should be a uri");
                }
                actual = uri;
            }
            return grainFactory.GetGrain<IUriRegistry>(actual);
        }

        public static async Task<Guid> LoadUserId(this IComaxGrainFactory grainFactory)
        {
            var _userID = await grainFactory.GetUriRegistry("").GetOrCreate();
            return _userID;
        }

        public static async Task<(AsyncEnumerableStream<MailMessage>, StreamSubscriptionHandle<MailMessage>)> FetchMail(this IComaxGrainFactory grainFactory, ILogger logger, string uri)
        {
            var mb = await grainFactory.GetEventMailbox(logger, uri);
            return await mb.GetStream();
        }

        public static async Task<IEventMailboxClient> GetEventMailbox(this IComaxGrainFactory grainFactory, ILogger logger, string uri)
        {
            var _userID = await grainFactory.GetUriRegistry(uri).GetOrCreate();
            var mb = await GetEventMailbox(grainFactory, logger, _userID);
            return mb;
        }

        public static async Task<IEventMailboxClient> GetEventMailbox(this IComaxGrainFactory grainFactory, ILogger logger, Guid? id = null)
        {
            Guid? userID = id ?? await grainFactory.LoadUserId();
            var gr = grainFactory.GetGrain<IEventMailbox>(userID.Value);
            var cl = new EventMailboxClusterClient(grainFactory, gr, logger);
            return cl;
        }

        
    }
}

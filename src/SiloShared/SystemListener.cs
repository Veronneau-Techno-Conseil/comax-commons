﻿using CommunAxiom.Commons.Client.Contracts;
using CommunAxiom.Commons.Client.Contracts.ComaxSystem;
using CommunAxiom.Commons.Client.SiloShared.Conf;
using CommunAxiom.Commons.Client.SiloShared.System;
using Microsoft.Extensions.Configuration;
using Orleans.Streams;
using System;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.SiloShared
{
    public class SystemListener: IAsyncDisposable, IDisposable
    {
        private bool _disposed;
        private IClusterManagement? clusterManagement;
        private readonly ICommonsClientFactory clientFactory;
        private (ICommonsClusterClient, StreamSubscriptionHandle<SystemEvent>) streamSubscriptionHandle;
        private readonly IConfiguration configuration;
        public SystemListener(IClusterManagement clusterManagement, ICommonsClientFactory clientFactory, IConfiguration configuration)
        {
            this.clusterManagement = clusterManagement;
            this.clientFactory = clientFactory;
            this.configuration = configuration;
        }

        ~SystemListener()
        {
            if (!_disposed)
            {
                this.Dispose();
            }
        }

        public async Task Listen()
        {
            streamSubscriptionHandle = await clientFactory.WithUnmanagedClient(cc=> cc.SubscribeSystem(OnsystemEvent, OnSystemStreamError, OnCompleted));
        }

        public async Task OnsystemEvent(SystemEvent systemEvent, StreamSequenceToken token)
        {
            switch (systemEvent.Type)
            {
                case SystemEventType.ClusterAuthenticationComplete:
                    await this.clientFactory.WithClusterClient(cl=> cl.SetCredentials(configuration));
                    await clusterManagement.SetSilo(Silos.Main);
                    break;
                case SystemEventType.ClusterAuthenticationReset:
                    await clusterManagement.SetSilo(Silos.Pilot);
                    break;
            }
        }
        
        public Task OnSystemStreamError(Exception e)
        {
            Console.Error.WriteLine(e.ToString());
            Console.Error.WriteLine(e.StackTrace);
            return Task.CompletedTask;
        }
        
        public Task OnCompleted()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            DisposeAsync().GetAwaiter().GetResult();
        }

        public async ValueTask DisposeAsync()
        {
            if (!this._disposed)
            {
                await this.streamSubscriptionHandle.Item2.UnsubscribeAsync();
                await this.streamSubscriptionHandle.Item1.Close();
                clusterManagement = null;
                this.streamSubscriptionHandle.Item1.Dispose();
                this.streamSubscriptionHandle = (null, null);
                this._disposed = true;
            }
        }
    }
}
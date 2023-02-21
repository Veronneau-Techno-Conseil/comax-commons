using Comax.Commons.ApiStorageProvider.Provider;
using Comax.Commons.StorageProvider.Hosting;
using Comax.Commons.StorageProvider.Models;
using Comax.Commons.StorageProvider.Serialization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Orleans;
using Orleans.Providers;
using Orleans.Runtime;
using Orleans.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Comax.Commons.StorageProvider
{
    public class DefaultStorageProvider : BaseProvider
    {
        private readonly string _name;
        private readonly ILogger<DefaultStorageProvider> _logger;
        
        private ISerializationProvider _serializationProvider;
        private readonly GrainStorageClientFactory _grainStorageClient;

        public DefaultStorageProvider(string name, ILogger<DefaultStorageProvider> logger, GrainStorageClientFactory grainStorageClient)
        {
            _name = name;
            _logger = logger;
            _grainStorageClient= grainStorageClient;
        }

        

        public override void Participate(ISiloLifecycle lifecycle)
        {
            lifecycle.Subscribe(OptionFormattingUtilities.Name<DefaultStorageProvider>(_name),
                                    ServiceLifecycleStage.RuntimeInitialize, Init);
        }
        public override async Task ClearStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            var cl = _grainStorageClient.Create();
            var ss = Uri.EscapeDataString(grainType.Substring(grainType.LastIndexOf(".") + 1)+grainState.Type.Name);
            var blobName = GetBlobName(ss, grainReference);

            if (await cl.Any(ss, blobName))
            {
                await cl.Delete(ss, blobName);
            }

            grainState.RecordExists = false;
            grainState.State = null;
        }

        public override async Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            var cl = _grainStorageClient.Create();
            var ss = Uri.EscapeDataString(grainType.Substring(grainType.LastIndexOf(".") + 1) + grainState.Type.Name);
            var blobName = GetBlobName(ss, grainReference);

            if(await cl.Any(ss, blobName))
            {
                var jo = await cl.GetValue(ss, blobName);

                grainState.State = jo.ToObject(grainState.Type);
                grainState.RecordExists = true;
            }
            else
            {
                grainState.RecordExists = false;
            }

            grainState.ETag = blobName;
        }

        public override async Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {

            var cl = _grainStorageClient.Create();
            var ss = Uri.EscapeDataString(grainType.Substring(grainType.LastIndexOf(".") + 1) + grainState.Type.Name);
            var blobName = GetBlobName(ss, grainReference);

            if(grainState.State == null)
            {
                await ClearStateAsync(ss, grainReference, grainState);
                return;
            }

            var jo = JObject.FromObject(grainState.State);
            await cl.UpsertValue(ss, blobName, jo);
            grainState.RecordExists = true;
            grainState.ETag = blobName;
        }


        public async Task Init(CancellationToken cancellationToken)
        {
            
        }
    }
}

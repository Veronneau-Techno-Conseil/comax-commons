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
    public class DefaultStorageProvider : IGrainStorage, ILifecycleParticipant<ISiloLifecycle>
    {
        private readonly string _name;
        private readonly ILogger<DefaultStorageProvider> _logger;
        
        private ISerializationProvider _serializationProvider;
        private readonly GrainStorageClient _grainStorageClient;

        public DefaultStorageProvider(string name, ILogger<DefaultStorageProvider> logger, GrainStorageClient grainStorageClient)
        {
            _name = name;
            _logger = logger;
            _grainStorageClient= grainStorageClient;
        }

        public static string GetBlobName(string grainType, GrainReference grainId)
        {
            return string.Format("{0}-{1}.json", grainType, grainId.ToKeyString());
        }

        public void Participate(ISiloLifecycle lifecycle)
        {
            lifecycle.Subscribe(OptionFormattingUtilities.Name<DefaultStorageProvider>(_name),
                                    ServiceLifecycleStage.RuntimeInitialize, Init);
        }
        public async Task ClearStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {

            var blobName = GetBlobName(grainType, grainReference);

            if (await _grainStorageClient.Any(grainType, blobName))
            {
                await _grainStorageClient.Delete(grainType, blobName);
            }

            grainState.RecordExists = false;
            grainState.State = null;
        }

        public async Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            var blobName = GetBlobName(grainType, grainReference);

            if(await _grainStorageClient.Any(grainType, blobName))
            {
                var jo = await _grainStorageClient.GetValue(grainType, blobName);

                grainState.State = jo.ToObject(grainState.Type);
                grainState.RecordExists = true;
            }
            else
            {
                grainState.RecordExists = false;
            }

            grainState.ETag = blobName;
        }

        public async Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {

            var blobName = GetBlobName(grainType, grainReference);

            if(grainState.State == null)
            {
                await ClearStateAsync(grainType, grainReference, grainState);
                return;
            }

            var jo = JObject.FromObject(grainState.State);
            await _grainStorageClient.UpsetValue(grainType, blobName, jo);
            grainState.RecordExists = true;
            grainState.ETag = blobName;
        }


        private async Task Init(CancellationToken cancellationToken)
        {
            
        }
    }
}

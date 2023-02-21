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
    public class JObjectStorageProvider : IGrainStorage, ILifecycleParticipant<ISiloLifecycle>
    {
        private readonly string _name;
        private readonly ILogger<JObjectStorageProvider> _logger;

        private ISerializationProvider _serializationProvider;
        private readonly GrainStorageClientFactory _grainStorageClient;

        public JObjectStorageProvider(string name, ILogger<JObjectStorageProvider> logger, GrainStorageClientFactory grainStorageClient)
        {
            _name = name;
            _logger = logger;
            _grainStorageClient = grainStorageClient;
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
            var cl = _grainStorageClient.Create();
            var blobName = GetBlobName(grainType, grainReference);

            if (await cl.Any(grainType, blobName))
            {
                await cl.Delete(grainType, blobName);
            }

            grainState.RecordExists = false;
            grainState.State = null;
        }

        public async Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            var cl = _grainStorageClient.Create();
            var blobName = GetBlobName(grainType, grainReference);

            if (await cl.Any(grainType, blobName))
            {
                var jo = await cl.GetValue(grainType, blobName);

                grainState.State = jo;
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

            var cl = _grainStorageClient.Create();
            var blobName = GetBlobName(grainType, grainReference);

            if (grainState.State == null)
            {
                await ClearStateAsync(grainType, grainReference, grainState);
                return;
            }

            var jo = (JObject)grainState.State;
            await cl.UpsertValue(grainType, blobName, jo);
            grainState.RecordExists = true;
            grainState.ETag = blobName;
        }

        private void Assign(object o, string property, object value)
        {
            o.GetType().GetProperty(property).SetValue(o, value, null);
        }

        private async Task Init(CancellationToken cancellationToken)
        {
           
        }
    }
}

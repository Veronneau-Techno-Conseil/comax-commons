﻿using Orleans.Runtime;
using Orleans.Storage;
using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Comax.Commons.StorageProvider;

namespace Comax.Commons.ApiStorageProvider.Provider
{
    public abstract class BaseProvider : IGrainStorage, ILifecycleParticipant<ISiloLifecycle>
    {
        protected readonly ApiStorageConfiguration _apiStorageConfiguration;
        public BaseProvider(ApiStorageConfiguration apiStorageConfiguration)
        {
            _apiStorageConfiguration = apiStorageConfiguration;
        }

        protected string GetBlobName(string grainType, GrainReference grainId)
        {
            return Uri.EscapeDataString($"{(string.IsNullOrWhiteSpace(_apiStorageConfiguration.SenderName)? "": _apiStorageConfiguration.SenderName + "-") }{grainType}-{grainId.ToKeyString()}.json");
        }

        protected string GetTypeKey(string grainType, IGrainState grainState)
        {
            var ss = Uri.EscapeDataString(grainType.Substring(grainType.LastIndexOf(".") + 1) + grainState.Type.Name);
            return ss;
        }

        public abstract Task ClearStateAsync(string grainType, GrainReference grainReference, IGrainState grainState);
        public abstract void Participate(ISiloLifecycle lifecycle);
        public abstract Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState);
        public abstract Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState);
    }
}

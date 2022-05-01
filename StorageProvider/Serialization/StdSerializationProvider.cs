using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Orleans;
using Orleans.Runtime;
using Orleans.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.StorageProvider.Serialization
{
    public class StdSerializationProvider : ISerializationProvider
    {
        private readonly SerializationManager _serializationManager;
        
        public StdSerializationProvider(SerializationManager serializationManager)
        {
            _serializationManager = serializationManager;
        }

        public void Configure(string name)
        {
            
        }

        public object Deserialize(byte[] obj)
        {
            return _serializationManager.DeserializeFromByteArray<object>(obj);
        }

        public byte[] Serialize(object obj)
        {
            return _serializationManager.SerializeToByteArray(obj);
        }
    }
}

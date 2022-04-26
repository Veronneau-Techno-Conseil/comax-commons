using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.StorageProvider.Serialization
{
    public interface ISerializationProvider
    {
        byte[] Serialize(object obj);
        object Deserialize(byte[] obj);
        void Configure(string name);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.StorageProvider.Models
{
    internal class GrainStorageModel
    {
        [LiteDB.BsonId]
        public LiteDB.ObjectId Id { get; set; }
        public byte[] Contents { get; set; }
        public string ETag { get; set; }
    }
}

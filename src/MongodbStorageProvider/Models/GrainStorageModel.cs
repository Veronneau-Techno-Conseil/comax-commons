using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace Comax.Commons.StorageProvider.Models
{
    internal class GrainStorageModel
    {       
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public MongoDB.Bson.BsonObjectId Id { get; set; }
        public byte[] Contents { get; set; }
        public string ETag { get; set; }
        
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.StorageProvider.Models
{
    public class BaseGrainStorageModel
    {
        [LiteDB.BsonId]
        public LiteDB.ObjectId Id { get; set; }
        public string ETag { get; set; }
    }

    public class GrainStorageModel<T>: BaseGrainStorageModel
    {
        public T Contents { get; set; }
    }

    //Todo: Test generic storage model
    public class GrainStorageModel : BaseGrainStorageModel
    {
        public byte[] Contents { get; set; }   
    }
}

using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestContracts
{
    public class Flat
    {
        [BsonId]
        public ObjectId? ObjectId { get; set; }
        public string? Name { get; set; }
        public DateTimeOffset DateTimeOffset { get; set; }
        public long LongValue { get; set; }
        public bool BoolValue { get; set; }
        public byte[]? Content { get; set; }
    }
}

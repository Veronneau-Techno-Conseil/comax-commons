using System;
using System.Threading.Tasks;

namespace GrainTests.Shared
{
    public class PersistentStorageMock<T> : Orleans.Runtime.IPersistentState<T> where T : class, new()
    {
        public T? StoredValue { get; set;  }

        public string Key { get; set; } = default!;

        public T State { get; set; } = new T();

        public string Etag => $"MemoryStorage-{Key}";

        public bool RecordExists => StoredValue != null;

        public Task ClearStateAsync()
        {
            StoredValue = null;
            State = default!;
            return Task.CompletedTask;
        }

        public Task ReadStateAsync()
        {
            State = StoredValue ?? default!;
            return Task.CompletedTask;
        }

        public Task WriteStateAsync()
        {
            StoredValue = State;
            return Task.CompletedTask;
        }
    }
}

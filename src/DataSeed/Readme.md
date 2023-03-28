# Data Seed

The DataSeed grain allows data transfer from the Storage grain to the source of the request.
Since all our data will be stored in the Storage grain. whenever a user makes a request to retrieve some data, the request will go through DataSeed grain which contacts Storage database for a specific record, then sends it through streams so that the DataChunk grain would be able to handle it in form of Rows.

We have used the already existing DataIndex in Storage grain to handle the data type.

    public class DataIndex
    {
        public DataIndex()
        {
            Keys = new List<string>();
            Index = new List<DataIndexItem>();
        }
        public Guid Id { get; set; }
        public List<string> Keys { get; set; }
        public List<DataIndexItem> Index { get; set; }
    }

    public class DataIndexItem
    {
        public string Id { get; set; }
        public Dictionary<string, string> IndexData { get; set; }
    }

The DataSeed grain will handle: 
- retrieval and preparation of indexes
- sending the indexes
- streaming the data from the Storage grain
- validating/fixing the data

<a href="https://imgbb.com/"><img src="https://i.ibb.co/6BhvQCp/dataseed.png" alt="dataseed" border="0"></a>


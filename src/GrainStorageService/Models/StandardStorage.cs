using Newtonsoft.Json.Linq;

namespace GrainStorageService.Models
{
    public class StandardStorage
    {
        public string Id { get; set; }
        public string EntityType { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public JObject Storage { get; set; }
    }
}

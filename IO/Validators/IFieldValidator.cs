using CommunAxiom.Commons.Ingestion.Configuration;
using Newtonsoft.Json.Linq;

namespace CommunAxiom.Commons.Ingestion.Validators
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFieldValidator
    {
        /// <summary>
        /// 
        /// </summary>
        string Tag { get; }

        /// <summary>
        ///
        /// </summary>
        string Parameter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        ValidationError Validate(DataSourceConfiguration configuration, JToken field);
    }
}
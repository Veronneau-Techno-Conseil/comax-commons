using CommunAxiom.Commons.Ingestion.Configuration;
using Newtonsoft.Json.Linq;

namespace CommunAxiom.Commons.Ingestion.DataSource
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDataSourceReader
    {
        /// <summary>
        /// 
        /// </summary>
        IngestionType IngestionType { get; }

        /// <summary>
        /// 
        /// </summary>
        IEnumerable<DataSourceConfiguration> ConfigurationFields { get; }

        /// <summary>
        /// 
        /// </summary>
        IEnumerable<FieldMetaData> DataDescription { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceConfig"></param>
        void Setup(SourceConfig sourceConfig = null);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<ValidationError> ValidateConfiguration();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Stream ReadData();

    }

}

using System;
using CommunAxiom.Commons.Ingestion.Configuration;
using CommunAxiom.Commons.Ingestion.DataSource;

namespace CommunAxiom.Commons.Ingestion.DataSource
{
	/// <summary>
    /// 
    /// </summary>
	public static class SourceFactory
	{
		/// <summary>
        /// 
        /// </summary>
        /// <param name="sourceType"></param>
		public static IDataSourceReader Create(DataSourceType sourceType)
		{
			return new TextDataSourceReader(null);
		}
	}
}


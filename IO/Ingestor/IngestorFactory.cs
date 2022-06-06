using System;
using CommunAxiom.Commons.Ingestion.Configuration;
using CommunAxiom.Commons.Ingestion.Injestor;

namespace CommunAxiom.Commons.Ingestion.Ingestor
{
	public static class IngestorFactory
	{
		public static IIngestor Create(IngestionType ingestionType)
		{
			return new JsonIngestor();
		}
	}
}


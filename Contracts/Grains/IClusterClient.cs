using CommunAxiom.Commons.Client.Contracts.Account;
using CommunAxiom.Commons.Client.Contracts.Datasource;
using CommunAxiom.Commons.Client.Contracts.DataTransfer;
using CommunAxiom.Commons.Client.Contracts.Ingestion;
using CommunAxiom.Commons.Client.Contracts.Portfolio;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Client.Contracts.Grains
{
    public interface IClusterClient
    {
        IAccount GetAccount();
        IDatasource GetDatasource(string datasourceId);
        IDataTransfer GetDataTransfer(string operationId);
        IIngestion GetIngestion(string ingestionId);
        IPortfolio GetPortfolio(string portfolioId);
    }
}

﻿using CommunAxiom.Commons.CommonsShared.Contracts.DataSeed;
using Orleans;
using Orleans.Runtime;
using System;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.DataSeedGrain
{
    public class DataSeed : Grain, IDataSeed
    {
        private readonly IPersistentState<DataSeedState> _dataSeedState;
        public DataSeed([PersistentState("dataSeedGrain")] IPersistentState<DataSeedState> dataSeedState)
        {
            _dataSeedState = dataSeedState;
        }
        public DataSeedBusiness _dataSeedBusiness;

        public async Task RetrieveData(Guid id)
        {
            var data =await _dataSeedBusiness.GetDataFromStorage("dsUri");
        }

        public Task SendIndex()
        {
            throw new NotImplementedException();
        }

        public Task UploadData(byte[] Data)
        {
            throw new NotImplementedException();
        }

        public Task Validate()
        {
            throw new NotImplementedException();
        }
    }
}

﻿using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Contracts.Replication
{
    public interface IReplication: IGrainWithStringKey
    {
        Task<string> TestGrain(string Grain);
    }
}

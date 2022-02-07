using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Contracts.Project
{
    public interface IProject: IGrainWithStringKey
    {
        Task<string> TestGrain(string Grain);
    }
}

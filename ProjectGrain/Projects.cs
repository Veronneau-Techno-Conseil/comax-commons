using System;
using Orleans;
using CommunAxiom.Commons.Client.Contracts.Project;
using System.Threading.Tasks;

namespace ProjectGrain
{
    public class Projects : Grain, IProject
    {
        public Task<string> TestGrain(string Grain)
        {
            return Task.FromResult($"The {Grain} grain has been launched. Check it on the dashboard");
        }
    }
}

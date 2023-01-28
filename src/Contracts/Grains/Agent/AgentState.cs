using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Client.Contracts.Grains.Agent
{
    public class AgentState
    {
        public AgentState() 
        { 
        }

        public List<UserAuthState> UserAuthStates { get; set; } = new List<UserAuthState>();
    }
}

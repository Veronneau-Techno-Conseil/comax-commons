using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Client.Contracts.Auth
{
    public enum Instruction
    {
        LaunchAuthStream = 0,
        OpenUrl= 1,
        SetResult = 2,
        FetchUser = 3,
    }
}

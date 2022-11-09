using Comax.Commons.Orchestrator.Contracts.PublicBoard;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.Orchestrator.MailboxGrain
{
    public class PublicBoardRepo
    {
        IPersistentState<PublicBoardIndex> _storageState;
        public PublicBoardRepo(IPersistentState<PublicBoardIndex> storageState)
        {
            _storageState = storageState;
        }
    }
}

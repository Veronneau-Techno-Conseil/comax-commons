using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.Orchestrator.Contracts.PublicBoard
{
    public class PublicBoardIndex
    {

        public class Item
        {
            public Guid Id { get; set; }
            public DateTime ReceivedDate { get; set; }
        }
    }
}

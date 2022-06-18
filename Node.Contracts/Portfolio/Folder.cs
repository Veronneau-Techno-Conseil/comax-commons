using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.Orchestrator.Contracts.Portfolio
{
    public class Folder
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public string Name { get; set; }
    }
}

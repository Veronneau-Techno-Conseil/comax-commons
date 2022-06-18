using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.Orchestrator.Contracts.Metadata
{
    public class Detail
    {
        public Guid Id { get; set; }
        public DetailType DetailType { get;set;}
        public string Title { get; set; }
        public string Content { get; set; }
        public string HtmlContent { get; set; }
    }
}

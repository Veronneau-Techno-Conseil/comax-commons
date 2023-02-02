using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Orleans
{
    public class StreamSpec
    {
        public Guid Id { get; set; }
        public string StreamProvider { get; set; }
        public string Namespace { get; set; }
    }
}

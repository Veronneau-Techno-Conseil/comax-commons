using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Client.Contracts.Remote
{
    public class BrowserOpts
    {
        public string StartUrl { get; set; }
        public string EndUrl { get; set; }
        public int DisplayMode { get; set; }
        public TimeSpan Timeout
        {
            get;
            set;
        } = TimeSpan.FromMinutes(5.0);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Client.Contracts.ComaxSystem
{
    public class Log
    {
        public long LogId { get; set; }
        public string LogLevel { get; set; }
        public string Source { get; set; }
        public string ExceptionMessage { get; set; }
        public string StackTrace { get; set; }
        public string CreatedDate { get; set; }
        public string UserId { get; set; }
        public string Client { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Client.Contracts
{
    public class OperationResult<TResult>
    {
        public TResult Result { get; set; }
        public bool IsError { get; set; }
        public string Error { get; set; }
        public string Detail { get; set; }
    }

    public class OperationResult
    {
        public bool IsError { get; set; }
        public string Error { get; set; }
        public string Detail { get; set; }
    }
}

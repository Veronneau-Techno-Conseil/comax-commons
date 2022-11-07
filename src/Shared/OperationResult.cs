using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Shared
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
        public const string ERR_UNEXP_NULL = "ERR_UNEXP_NULL";
        public const string ERR_UNEXP_ERR = "ERR_UNEXP_ERR";
        public const string PARAM_ERR = "PARAM_ERR";
        public bool IsError { get; set; }
        public string Error { get; set; }
        public string Detail { get; set; }
    }
}

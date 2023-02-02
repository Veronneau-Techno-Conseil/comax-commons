using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Shared
{
    public class OperationResult<TResult>: OperationResult
    {
        public TResult? Result { get; set; } = default;

        public OperationResult()
        {

        }

        public OperationResult(OperationResult other)
        {
            IsError = other.IsError;
            Error = other.Error;
            Detail = other.Detail;
            Exception = other.Exception;
        }
    }

    public class OperationResult
    {
        public const string ERR_UNEXP_NULL = "ERR_UNEXP_NULL";
        public const string ERR_UNEXP_ERR = "ERR_UNEXP_ERR";
        public const string PARAM_ERR = "PARAM_ERR";
        public bool IsError { get; set; }
        public string Error { get; set; }
        public string Detail { get; set; }
        public Exception Exception { get; set; }
    }
}

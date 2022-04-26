using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.IO.Configuration
{
    public class ValidationError
    {
        public const string MANDATORY = "MANDATORY";

        public string ErrorCode { get; set; }
        public string FieldName { get; set; }
    }
}

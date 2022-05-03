using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Client.Contracts.Remote
{
    public class BrowserResult
    {
        //
        // Summary:
        //     Gets or sets the type of the result.
        //
        // Value:
        //     The type of the result.
        public int ResultType
        {
            get;
            set;
        }

        //
        // Summary:
        //     Gets or sets the response.
        //
        // Value:
        //     The response.
        public string Response
        {
            get;
            set;
        }

        //
        // Summary:
        //     Gets or sets the error.
        //
        // Value:
        //     The error.
        public string Error
        {
            get;
            set;
        }

        //
        // Summary:
        //     Gets or sets the error description.
        //
        // Value:
        //     The error description.
        public string ErrorDescription
        {
            get;
            set;
        }
    }
}

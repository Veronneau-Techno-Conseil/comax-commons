using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Orleans.Security
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AuthorizePassthrough: Attribute
    {
    }
}

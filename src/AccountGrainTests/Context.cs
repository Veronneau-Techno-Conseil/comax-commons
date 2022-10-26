using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountGrainTests
{
    public static class Context
    {
        public static IConfiguration? Configuration { get; set; }
    }
}

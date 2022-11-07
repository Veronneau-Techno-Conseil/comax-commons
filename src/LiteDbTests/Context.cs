using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteDbTests
{
    public static class Context
    {
        public static IConfiguration? Configuration { get; set; }
        public static IServiceProvider ServiceProvider { get; set; }
    }
}

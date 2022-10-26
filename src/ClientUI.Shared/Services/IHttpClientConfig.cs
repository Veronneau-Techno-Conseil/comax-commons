using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.ClientUI.Shared.Services
{
    public interface IHttpClientConfig
    {
        Task Configure(HttpClient client);
    }
}

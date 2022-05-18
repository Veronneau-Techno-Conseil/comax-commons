using ClientUI.Shared.Models;
using CommunAxiom.Commons.Client.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientUI.Shared.ViewModels.Interfaces
{
    public interface ILoginViewModel
    {
        public string ClientId { get; set; }
        public string Secret { get; set; }
        public Task Login(LoginType loginType);
        public Task<OperationResult<string>> GetState();
    }
}

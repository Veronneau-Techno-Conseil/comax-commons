using CommunAxiom.Commons.ClientUI.Shared.Models;
using CommunAxiom.Commons.Client.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunAxiom.Commons.Shared;

namespace CommunAxiom.Commons.ClientUI.Shared.ViewModels.Interfaces
{
    public interface ISessionViewModel
    {
        string ClientId { get; set; }
        string Secret { get; set; }
        Task<User?> Login();
        Task<OperationResult<string>> GetState();
        Task<User?> GetUserByJWTAsync();
    }
}

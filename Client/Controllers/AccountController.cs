using ClusterClient;
using CommunAxiom.Commons.Client.Contracts.Account;
using CommunAxiom.Commons.Client.Contracts.ComaxSystem;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Orleans;
using System;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.ClientUI.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {

        private readonly ICommonsClientFactory _clientFactory;
        public AccountController(ICommonsClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet("Get")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _clientFactory.WithClusterClient(c=> Task.FromResult(c.GetAccount().GetGrainIdentity().IdentityString)));
        }

        [HttpPost()]
        public async Task<IActionResult> SetDetails(AccountDetails account)
        {
            await _clientFactory.WithClusterClient(c => c.GetAccount().Initialize(account));
            
            return Ok();
        }

        [HttpGet("GetDetails/{GrainId}")]
        public async Task<IActionResult> GetDetails(string GrainId)
        {
            var details =  await _clientFactory.WithClusterClient(c => c.GetAccount().GetDetails());
            details.ClientSecret = String.Empty;
            return Ok(details);
        }

        [HttpGet("security_check")]
        public async Task<IActionResult> security_check()
        {
            await _clientFactory.WithClusterClient(c => c.GetAccount().SecurityCheck());
            return Ok();
        }
    }
}

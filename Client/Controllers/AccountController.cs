using CommunAxiom.Commons.Client.Contracts.Account;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Orleans;
using System;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.ClientUI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {

        private readonly IClusterClient _clusterClient;
        public AccountController(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }

        [HttpGet("Get/{GrainId}")]
        public async Task<IActionResult> Get(string GrainId)
        {

            var result = _clusterClient.GetGrain<IAccount>(Guid.Empty).GetGrainIdentity().IdentityString;

            return Ok(result);
        }

        [HttpPost("SetDetails/{GrainId}")]
        public async Task<IActionResult> SetDetails(string GrainId, AccountDetails account)
        {
            await _clusterClient.GetGrain<IAccount>(Guid.Empty).Initialize(account);

            return Ok();
        }

        [HttpGet("GetDetails/{GrainId}")]
        public async Task<IActionResult> GetDetails(string GrainId)
        {

            var result = await _clusterClient.GetGrain<IAccount>(Guid.Empty).GetDetails();

            return Ok(result);
        }
    }
}

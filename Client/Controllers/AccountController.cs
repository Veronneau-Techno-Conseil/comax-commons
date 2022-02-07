using CommunAxiom.Commons.Client.Contracts.Account;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Orleans;
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

            var result = await _clusterClient.GetGrain<IAccount>(GrainId).TestGrain(GrainId);

            return Ok(result);
        }

        [HttpPost("SetDetails/{GrainId}")]
        public async Task<IActionResult> SetDetails(string GrainId, [FromBody] object account)
        {

            var AccountJSON = JObject.Parse(account.ToString());

            var accountDetails = new AccountDetails
            {
                ApplicationId = AccountJSON["ApplicationID"].ToString(),
                ClientID = AccountJSON["ClientID"].ToString(),
                ClientSecret = AccountJSON["ClientSecret"].ToString(),
                AccountsToken = AccountJSON["AccessToken"].ToString()
            };           

            var result = await _clusterClient.GetGrain<IAccount>(GrainId).SetDetails(GrainId,accountDetails);

            return Ok(result);
        }

        [HttpGet("GetDetails/{GrainId}")]
        public async Task<IActionResult> GetDetails(string GrainId)
        {

            var result = await _clusterClient.GetGrain<IAccount>(GrainId).GetDetails(GrainId);

            return Ok(result);
        }
    }
}

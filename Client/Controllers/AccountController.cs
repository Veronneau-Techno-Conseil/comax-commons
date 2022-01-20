using CommunAxiom.Commons.Client.Contracts.Account;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _clusterClient.GetGrain<IAccount>(0).TestGrain("Account");

            return Ok(result);
        }

    }
}

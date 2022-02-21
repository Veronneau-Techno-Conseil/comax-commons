using CommunAxiom.Commons.Client.Contracts.Replication;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.ClientUI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReplicationController : ControllerBase
    {
        private readonly IClusterClient _clusterClient;
        public ReplicationController(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _clusterClient.GetGrain<IReplication>("Replication").TestGrain("Replication");

            return Ok(result);
        }

    }
}

using CommunAxiom.Commons.Client.Contracts;
using CommunAxiom.Commons.Client.Contracts.ComaxSystem;
using CommunAxiom.Commons.Client.Contracts.Replication;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.ClientUI.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReplicationController : ControllerBase
    {
        private readonly ICommonsClientFactory _clusterClient;
        public ReplicationController(ICommonsClientFactory clusterClient)
        {
            _clusterClient = clusterClient;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _clusterClient.WithClusterClient(cc=>cc.GetReplication().TestGrain("Replication"));

            return Ok(result);
        }

    }
}

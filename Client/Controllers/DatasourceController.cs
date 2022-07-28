using CommunAxiom.Commons.Client.Contracts.Datasource;
using CommunAxiom.Commons.Client.Contracts.IO;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.ClientUI.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatasourceController : ControllerBase
    {
        private readonly IClusterClient _clusterClient;
        public DatasourceController(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _clusterClient.GetGrain<IDatasource>(id).GetConfig();
            return Ok(result);
        }

        [HttpPost]
        [HttpPut]
        public async Task<IActionResult> Set(string id, SourceState sourceState)
        {
            await _clusterClient.GetGrain<IDatasource>(id).SetConfig(sourceState);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            await _clusterClient.GetGrain<IDatasource>(id).DeleteConfig();
            return Ok();
        }
    }
}

using CommunAxiom.Commons.Client.Contracts.Datasource;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.ClientUI.Controllers
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
        public async Task<IActionResult> Get()
        {
            var result = await _clusterClient.GetGrain<IDatasource>(0).TestGrain("Datasource");

            return Ok(result);
        }

    }
}

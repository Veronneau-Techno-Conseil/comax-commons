using CommunAxiom.Commons.Client.Contracts.DataTransfer;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.ClientUI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class DataTransferController : Controller
    {
        private readonly IClusterClient _clusterClient;
        public DataTransferController(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _clusterClient.GetGrain<IDataTransfer>("DataTransfer").TestGrain("DataTransfer");

            return Ok(result);
        }
    }
}

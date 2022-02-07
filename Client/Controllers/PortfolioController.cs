using Microsoft.AspNetCore.Mvc;
using Orleans;
using System.Threading.Tasks;
using CommunAxiom.Commons.Client.Contracts.Portfolio;

namespace CommunAxiom.Commons.ClientUI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class PortfolioController : ControllerBase
    {
        private readonly IClusterClient _clusterClient;
        public PortfolioController(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _clusterClient.GetGrain<IPortfolio>("Portfolio").TestGrain("Portfolio");

            return Ok(result);
        }

    }
}

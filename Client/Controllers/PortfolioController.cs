using Microsoft.AspNetCore.Mvc;
using Orleans;
using System.Threading.Tasks;
using CommunAxiom.Commons.Client.Contracts.Portfolio;
using Newtonsoft.Json.Linq;
using Orleans.GrainDirectory;

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

        [HttpGet("Get/{GrainId}")]
        public async Task<IActionResult> Get(string GrainId)
        {
            var result = await _clusterClient.GetGrain<IPortfolio>(GrainId).TestGrain(GrainId);

            return Ok(result);
        }

        [HttpPost("create/{GrainId}")]
        public async Task<IActionResult> CreatePortfolio(string GrainId, [FromBody] object Portfolio)
        {
            var PortfolioJSON = JObject.Parse(Portfolio.ToString());

            var PortfolioDetails = new PortfolioDetails
            {
                PortfolioID = PortfolioJSON["PortfolioID"].ToString(),
                PortfolioName = PortfolioJSON["PortfolioName"].ToString()
            };

            var result = await _clusterClient.GetGrain<IPortfolio>(GrainId).CreatePortfolio(GrainId, PortfolioDetails);

            return Ok(result);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            return Ok();
        }
    }
}

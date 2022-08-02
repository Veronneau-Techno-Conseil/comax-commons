using Microsoft.AspNetCore.Mvc;
using CommunAxiom.Commons.Client.Contracts.ComaxSystem;
using CommunAxiom.Commons.Client.Contracts.Grains.Portfolio;

namespace CommunAxiom.Commons.ClientUI.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class PortfolioController : ControllerBase
    {
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] PortfolioModel model, [FromServices] ICommonsClientFactory clusterClient)
        {
            try
            {
                if (model != null)
                {
                    var portfolio = new Portfolio
                    {
                        ID = model.ID,
                        Name = model.Name,
                        TheType = model.TheType,
                        ParentId = model.ParentId
                    };
                    await clusterClient.WithClusterClient(async cc =>
                    {
                        await cc.GetPortfolio("Portfolios").AddAPortfolio(portfolio);
                    });
                    return Ok();
                }
                Console.WriteLine("Model Error");
                return StatusCode(400);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(400);
            }
        }

        [HttpGet("GetAll")]
        public async Task<IEnumerable<Portfolio>> GetAll([FromServices] ICommonsClientFactory clusterClient)
        {
            var PortfoliosFinalList = await clusterClient.WithClusterClient(async cc =>
            {
                return await cc.GetPortfolio("Portfolios").GetPortfoliosList();
            });
            return PortfoliosFinalList;
        }

        [HttpGet("CheckName/{name}")]
        public async Task<bool> CheckIfNameExist([FromServices] ICommonsClientFactory clusterClient, [FromRoute] string name)
        {
            if (name != null)
            {
                var result = await clusterClient.WithClusterClient(async cc =>
                {
                    return await cc.GetPortfolio("Portfolios").CheckIfUnique(name);
                });
                return result;
            }
            return false;
        }

        public record class PortfolioModel(
            string ID,
            string Name,
            string TheType,
            string ParentId);
    }
}

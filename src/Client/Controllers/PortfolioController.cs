using Microsoft.AspNetCore.Mvc;
using CommunAxiom.Commons.Client.Contracts.ComaxSystem;
using CommunAxiom.Commons.Client.Contracts.Grains.Portfolio;
using CommunAxiom.Commons.ClientUI.Shared.Models;

namespace CommunAxiom.Commons.ClientUI.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PortfolioController : ControllerBase
    {
        private readonly ICommonsClientFactory _clusterClient;

        public PortfolioController(ICommonsClientFactory clusterClient)
        {
            this._clusterClient = clusterClient;
        }


        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] PortfolioModel model)
        {
            try
            {
                if (model != null)
                {
                    var portfolio = new PortfolioItem
                    {
                        ID = model.ID,
                        Name = model.Name,
                        Type = model.Type,
                        ParentId = model.ParentId
                    };
                    await _clusterClient.WithClusterClient(async cc =>
                    {
                        var pf = cc.GetPortfolio();
                        await pf.AddPortfolio(portfolio);
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
        public async Task<IEnumerable<PortfolioModel>> GetAll()
        {
            var PortfoliosFinalList = await _clusterClient.WithClusterClient(async cc =>
            {
                return await cc.GetPortfolio().GetPortfoliosList();
            });
            return PortfoliosFinalList?.Select(o => new PortfolioModel { ID =  o.ID, Name = o.Name, Type = o.Type, ParentId = o.ParentId });
        }

        [HttpGet("CheckName/{name}")]
        public async Task<bool> CheckIfNameExist([FromRoute] string name)
        {
            if (name != null)
            {
                var result = await _clusterClient.WithClusterClient(async cc =>
                {
                    return await cc.GetPortfolio().CheckIfUnique(name);
                });
                return result;
            }
            return false;
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using CommunAxiom.Commons.Client.Contracts.ComaxSystem;
using CommunAxiom.Commons.Client.Contracts.Grains.Portfolio;
using CommunAxiom.Commons.ClientUI.Shared.ViewModels.Interfaces;
using CommunAxiom.Commons.ClientUI.Shared.ViewModels;
using CommunAxiom.Commons.ClientUI.Shared.Models;

namespace CommunAxiom.Commons.ClientUI.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public partial class PortfolioController : ControllerBase
    {
        private readonly ICommonsClientFactory clusterClient;

        public PortfolioController(ICommonsClientFactory clusterClient)
        {
            this.clusterClient = clusterClient;
        }


        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] PortfolioModel model)
        {
            try
            {
                if (model != null)
                {
                    var portfolio = new Portfolio
                    {
                        ID = model.ID,
                        Name = model.Name,
                        Type = model.Type,
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
        public async Task<IEnumerable<PortfolioModel>> GetAll()
        {
            var PortfoliosFinalList = await clusterClient.WithClusterClient(async cc =>
            {
                return await cc.GetPortfolio("Portfolios").GetPortfoliosList();
            });
            return PortfoliosFinalList?.Select(o => new PortfolioModel { ID =  o.ID, Name = o.Name, Type = o.Type, ParentId = o.ParentId });
        }

        [HttpGet("CheckName/{name}")]
        public async Task<bool> CheckIfNameExist([FromRoute] string name)
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
    }
}

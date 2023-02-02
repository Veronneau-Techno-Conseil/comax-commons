using CommunAxiom.Commons.Client.Contracts.ComaxSystem;
using CommunAxiom.Commons.Client.Contracts.Grains.Explorer;
using CommunAxiom.Commons.ClientUI.Shared.Models;
using CommunAxiom.Commons.Shared;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.Core.Operations;
using Orleans;

namespace CommunAxiom.Commons.ClientUI.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExploreController : ControllerBase
    {
        private readonly ICommonsClientFactory _clientFactory;

        public ExploreController(ICommonsClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet()]
        public async Task<IActionResult> Index()
        {
            OperationResult result = null;
            try
            {
                var res = await _clientFactory.WithClusterClient(async cl =>
                {
                    var explorer = cl.GetExplorer();
                    return await explorer.GetPortfolios();
                });
                result = new OperationResult<List<SharedPortfolio>>()
                {
                    IsError = false,
                    Result = res
                };
                return Ok(result);
            }
            catch (Exception ex)
            {
                result = new OperationResult()
                {
                    IsError = true,
                    Error = OperationResult.ERR_UNEXP_ERR,
                    Detail = ex.Message,
                    Exception = ex
                };
                return BadRequest(result);
            }
        }

        [HttpGet("details/{portfolioUri}")]
        public async Task<IActionResult> LoadDetails(string portfolioUri)
        {
            OperationResult result = null;
            try
            {
                var paramUri = Uri.UnescapeDataString(portfolioUri);
                var res = await _clientFactory.WithClusterClient(async cl =>
                {
                    SharedPortfolioDetails result = new SharedPortfolioDetails();
                    var explorer = cl.GetExplorer();
                    var lst = await explorer.GetPortfolios();
                    var p = lst.First(x => x.PortfolioUri == paramUri);
                    result.SharedPortfolio = p;
                    var pl = await explorer.GetPortfolio(paramUri);
                    result.Entries = pl;
                    return result;
                });
                result = new OperationResult<SharedPortfolioDetails>()
                {
                    IsError = false,
                    Result = res
                };
                return Ok(result);
            }
            catch (Exception ex)
            {
                result = new OperationResult()
                {
                    IsError = true,
                    Error = OperationResult.ERR_UNEXP_ERR,
                    Detail = ex.Message,
                    Exception = ex
                };
                return BadRequest(result);
            }
        }


    }
}

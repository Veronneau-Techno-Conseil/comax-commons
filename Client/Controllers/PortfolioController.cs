using Microsoft.AspNetCore.Mvc;
using Orleans;
using System.Threading.Tasks;
using CommunAxiom.Commons.Client.Contracts.Portfolio;
using System.Text.Json;
using Orleans.GrainDirectory;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System;

namespace CommunAxiom.Commons.ClientUI.Server.Controllers
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

        [HttpPost("create/{GrainId}")]
        public async Task<IActionResult> CreatePortfolio([FromBody] object Portfolio)
        {
            //if not created, create the portfolios grain
            if (!await _clusterClient.GetGrain<IPortfolio>("Portfolios").ListIsSet())
            {
                await _clusterClient.GetGrain<IPortfolio>("Portfolios").CreatePortfoliosList();
            }

            //set the portfolio ready and extract the details
            var PortfolioJSON = JObject.Parse(Portfolio.ToString());

            var PortfolioDetails = new PortfolioDetails
            {
                ID = PortfolioJSON["ID"].ToString(),
                Name = PortfolioJSON["Name"].ToString(),
                Type = PortfolioJSON["Type"].ToString(),
                ParentId = PortfolioJSON["ParentId"].ToString()
            };

            //Add the portfolio created to the existing portfolioList
            await _clusterClient.GetGrain<IPortfolio>("Portfolios").AddAPortfolio(PortfolioDetails);

            //Get the updated List
            var PortfoliosFinalList = await _clusterClient.GetGrain<IPortfolio>("Portfolios").GetListDetails();

            return Ok(PortfoliosFinalList.Portfolios);
        }
    }
}

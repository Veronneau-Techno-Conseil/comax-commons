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
using CommunAxiom.Commons.Client.Contracts.ComaxSystem;
using CommunAxiom.Commons.ClientUI.Server.Helper;

namespace CommunAxiom.Commons.ClientUI.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class PortfolioController : ControllerBase
    {
        private readonly ITempData _tempData;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public PortfolioController(ITempData tempData, IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _tempData = tempData;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        //The below may be removed after shifting the concept from API to services

        //[HttpPost("create")]
        //public async Task<IActionResult> Create([FromServices] ICommonsClientFactory clusterClient, [FromBody] object Portfolio)
        //{
        //    //if not created, create the PortfoliosList
        //    var state = await clusterClient.WithClusterClient(async cc =>
        //    {
        //        var portfolio = cc.GetPortfolio("Portfolios");
        //        return await portfolio.ListIsSet();
        //    });
        //    if (state == false)
        //    {
        //        await clusterClient.WithClusterClient(async cc =>
        //        {
        //            var createList = await cc.GetPortfolio("Portfolios").CreatePortfoliosList();
        //        });
        //    }
        //    //Set the incoming portfolio ready
        //    var portfolioObject = JObject.Parse(Portfolio.ToString());
        //    //check validation. the ==null validation does not fit here
        //    var PortfolioDetails = new Portfolio
        //    {
        //        ID = portfolioObject["id"] == null ? "" : portfolioObject["id"].ToString(),
        //        Name = portfolioObject["name"] == null ? "" : portfolioObject["name"].ToString(),
        //        TheType = portfolioObject["theType"] == null ? "" : portfolioObject["theType"].ToString(),
        //        ParentId = portfolioObject["parentId"] == null ? "-1" : portfolioObject["parentId"].ToString()
        //    };
        //    //Add the portfolio created to the existing portfolioList
        //    if (PortfolioDetails != null)
        //    {
        //        await clusterClient.WithClusterClient(async cc =>
        //        {
        //            var portfolioCreated = await cc.GetPortfolio("Portfolios").AddAPortfolio(PortfolioDetails);
        //        });
        //    }
        //    //Get the updated List
        //    var PortfoliosFinalList = await clusterClient.WithClusterClient(async cc =>
        //    {
        //        return await cc.GetPortfolio("Portfolios").GetListDetails();
        //    });
        //    foreach(var a in PortfoliosFinalList.Portfolios)
        //    {
        //        Console.WriteLine("ID: " + a.ID + ". Name: " + a.Name + ". Type: " + a.TheType + ". ParentId" + a.ParentId);
        //    }
        //    return Ok(PortfoliosFinalList);
        //}

        //[HttpGet("GetAll")]
        //public async Task<PortfoliosList> GetAllPortfolios([FromServices] ICommonsClientFactory clusterClient)
        //{
        //    //if not created, create the PortfoliosList
        //    var state = await clusterClient.WithClusterClient(async cc =>
        //    {
        //        var portfolio = cc.GetPortfolio("Portfolios");
        //        return await portfolio.ListIsSet();
        //    });
        //    if (state == false)
        //    {
        //        await clusterClient.WithClusterClient(async cc =>
        //        {
        //            var createList = await cc.GetPortfolio("Portfolios").CreatePortfoliosList();
        //        });
        //    }
        //    //Get the updated List
        //    var PortfoliosFinalList = await clusterClient.WithClusterClient(async cc =>
        //    {
        //        return await cc.GetPortfolio("Portfolios").GetListDetails();
        //    });
        //    return PortfoliosFinalList;
        //}
    }
}

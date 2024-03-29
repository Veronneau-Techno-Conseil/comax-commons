﻿using CommunAxiom.Commons.Client.Contracts.Ingestion;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.ClientUI.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IngestionController : Controller
    {
        private readonly IClusterClient _clusterClient;
        public IngestionController(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _ = _clusterClient.GetGrain<IIngestion>("Ingestion").Run();

            return Ok();
        }
    }
}

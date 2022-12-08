using CommunAxiom.Commons.Client.Contracts.ComaxSystem;
using CommunAxiom.Commons.Client.Contracts.Ingestion;
using Microsoft.AspNetCore.Mvc;
using Orleans;

//This class has been added just for testing purposes and shall be deleted later on

namespace CommunAxiom.Commons.ClientUI.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ICommonsClientFactory _clusterClient;
        public TestController(ICommonsClientFactory clusterClient)
        {
            _clusterClient = clusterClient;
        }
        
        [HttpGet("TestDispatch")]
        public async Task<IActionResult> Get()
        {
            await _clusterClient.WithClusterClient(async cc =>
            {
                await cc.GetIngestion("0a3905f7-0e1b-423e-ac4a-a84d2784adfb").Run();
            });
            
            return Ok();
        }


        [HttpGet("TestDataState")]
        public async Task<IActionResult> TestDataState()
        {
            await _clusterClient.WithClusterClient(async cc =>
            {
                await cc
                    .GetDateStateMonitorSupervisor()
                    .RegisterAsync("0a3905f7-0e1b-423e-ac4a-a84d2784adfb");
            });
            
            return Ok();
        }
        
        [HttpGet("TestDataState1")]
        public async Task<IActionResult> TestDataState1()
        {
            await _clusterClient.WithClusterClient(async cc =>
            {
                await cc
                    .GetDateStateMonitorSupervisor()
                    .UnregisterAsync("0a3905f7-0e1b-423e-ac4a-a84d2784adfb");
            });
            
            return Ok();
        }
    }
}

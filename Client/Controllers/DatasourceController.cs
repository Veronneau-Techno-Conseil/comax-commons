using CommunAxiom.Commons.Client.Contracts.Datasource;
using CommunAxiom.Commons.Client.Contracts.IO;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using System.Threading.Tasks;
using CommunAxiom.Commons.Client.Contracts.ComaxSystem;
using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;
using CommunAxiom.Commons.ClientUI.Shared.Models;

namespace CommunAxiom.Commons.ClientUI.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatasourceController : ControllerBase
    {
        private readonly ICommonsClientFactory _clusterClient;

        public DatasourceController(ICommonsClientFactory clusterClient)
        {
            this._clusterClient = clusterClient;
        }

        [HttpGet("{id}")] 
        public async Task<IActionResult> Get([FromRoute]  string id)
        {
            SourceState result = null;
            await _clusterClient.WithClusterClient(async cc =>
            {
                result = await cc.GetDatasource(id).GetConfig();
            });
            return Ok(result);
        }

        [HttpPost("SetConfigurations")]
        public async Task<IActionResult> SetConfigurations(CreateConfigRequest request)
        {
            await _clusterClient.WithClusterClient(async cc =>
            {
                await cc.GetDatasource(request.Id).SetConfig(request.Config.DataSourceType, request.Config.Configurations);
            });
            return Ok();
        }
        
        [HttpPost("SetFieldMetaData")]
        public async Task<IActionResult> SetFieldMetaData(CreateFieldMetaDataRequest request)
        {
            await _clusterClient.WithClusterClient(async cc =>
            {
                await cc.GetDatasource(request.Id).SetFieldMetaData(request.Fields);
            });
            
            return Ok();
        }
        
        [HttpGet("GetFieldMetaData")]
        public async Task<IActionResult> GetFieldMetaData([FromQuery] string id)
        {
            List<FieldMetaData> result = null;
            
            await _clusterClient.WithClusterClient(async cc =>
            {
                result = await cc.GetDatasource(id).GetFieldMetaData();
            });
            
            return Ok(result);
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            await _clusterClient.WithClusterClient(async cc =>
            {
                await cc.GetDatasource(id).DeleteConfig();
            });
            return Ok();
        }
    }
}

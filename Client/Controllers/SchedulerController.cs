using CommunAxiom.Commons.Client.Contracts.ComaxSystem;
using CommunAxiom.Commons.Client.Contracts.Grains.Scheduler;
using Microsoft.AspNetCore.Mvc;

namespace CommunAxiom.Commons.ClientUI.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SchedulerController : ControllerBase
    {
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] SchedulerModel model, [FromServices] ICommonsClientFactory clusterClient)
        {
            try
            {
                if (model != null)
                {
                    var scheduler = new Schedulers
                    {
                        ID = model.ID,
                        DataSourceID = model.DataSourceID,
                        CronExpression = model.CronExpression,
                        CronTimeZone = model.CronTimeZone,
                        NextExecutionTime = model.NextExecutionTime,
                        ScheduleType = model.ScheduleType
                    };
                    await clusterClient.WithClusterClient(async cc =>
                        {
                            await cc.GetScheduler(0).AddAScheduler(scheduler);
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
        public async Task<IEnumerable<Schedulers>> GetAll([FromServices] ICommonsClientFactory clusterClient)
        {
            var SchedulersFinalList = await clusterClient.WithClusterClient(async cc =>
            {
                return await cc.GetScheduler(0).GetAllSchedulers();
            });
            return SchedulersFinalList;
        }

        public record class SchedulerModel(
            string ID,
            string DataSourceID,
            string CronExpression,
            string CronTimeZone,
            DateTime NextExecutionTime,
            string ScheduleType,
            DateTime StartDate);
    }
}

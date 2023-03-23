using GrainStorageService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace GrainStorageService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StateController : ControllerBase
    {
        private readonly StorageDbContext _storageDbContext;
        public StateController(StorageDbContext storageDbContext)
        {
            _storageDbContext = storageDbContext;
        }

        [HttpGet("Any/{objType}/{id}")]
        public async Task<IActionResult> Any(string objType, string id)
        {
            var coll = _storageDbContext.Set<StandardStorage>();
            var res = await coll.AnyAsync(x=> x.Id == id && x.EntityType.ToLower() == objType.ToLower());

            if(res)
                return Ok();
            return NotFound();
        }

        [HttpPost("{objType}/{id}")]
        public async Task<IActionResult> Upsert(string objType, string id, [FromBody] object data)
        {
            var str = data.ToString();

            try
            {
                JObject jo = JObject.Parse(str);
                var coll = _storageDbContext.Set<StandardStorage>();
                var res = coll.Where(x=>x.EntityType.ToLower() == objType.ToLower() && x.Id == id).FirstOrDefault();
                if (res != null)
                {
                    res.Storage = jo;
                    res.LastModifiedDate = DateTime.UtcNow;
                }
                else
                {
                    res = new StandardStorage
                    {
                        CreatedDate = DateTime.UtcNow,
                        LastModifiedDate = DateTime.UtcNow,
                        EntityType = objType.ToLower(),
                        Id = id,
                        Storage = jo
                    };
                    coll.Add(res);
                }
                await _storageDbContext.SaveChangesAsync();

                return Ok();

            }
            catch (Exception ex)
            {
                return this.UnprocessableEntity();
            }
        }

        [HttpDelete("{objType}/{id}")]
        public async Task<IActionResult> Delete(string objType, string id)
        {
            
            try
            {
                var coll = _storageDbContext.Set<StandardStorage>();
                var res = coll.Where(x => x.EntityType.ToLower() == objType.ToLower() && x.Id == id).FirstOrDefault();
                if (res == null)
                    return NotFound();

                coll.Remove(res);

                await _storageDbContext.SaveChangesAsync();

                return Ok();

            }
            catch (Exception ex)
            {
                return this.UnprocessableEntity();
            }
        }

        [HttpGet("{objType}/{id}")]
        public async Task<IActionResult> Get(string objType, string id)
        {

            try
            {
                var coll = _storageDbContext.Set<StandardStorage>();
                var res = coll.Where(x => x.EntityType.ToLower() == objType.ToLower() && x.Id == id).FirstOrDefault();
                if (res == null)
                    return NotFound();

                return Ok(res.Storage.ToString());
            }
            catch (Exception ex)
            {
                return this.UnprocessableEntity();
            }
        }
    }
}

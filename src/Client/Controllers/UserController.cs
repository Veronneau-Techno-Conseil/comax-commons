using CommunAxiom.Commons.ClientUI.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunAxiom.Commons.ClientUI.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            User u = new User()
            {
                Name = this.User.Identity.Name,
                Email = this.User.Identity.Name,
                Claims = this.User.Claims.Select(x => new Claim { Type = x.Type, Value = x.Value }).ToList(),
                Expires = DateTimeOffset.FromUnixTimeSeconds(long.Parse(this.User.Claims.First(x=>x.Type == "exp").Value)).DateTime,
                Id = this.User.Claims.First(x=>x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value
            };
            return  Ok(u);
        }
    }
}

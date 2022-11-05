using Microsoft.AspNetCore.Mvc;

namespace Referee.Controllers
{
  public class MembershipController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }
  }
}

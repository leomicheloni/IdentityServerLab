using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebClient
{
    public class HomeController: Controller
    {
        [Authorize]
        public IActionResult Index()
        {

            return View();
        }
    }
}

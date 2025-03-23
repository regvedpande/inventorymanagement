using Microsoft.AspNetCore.Mvc;

namespace RegvedInventoryDB.Controllers
{
    [Route("")] // Root URL
    public class HomeController : Controller
    {
        [HttpGet("")] // Maps to https://localhost:7124/
        public IActionResult Index()
        {
            return View();
        }
    }
}
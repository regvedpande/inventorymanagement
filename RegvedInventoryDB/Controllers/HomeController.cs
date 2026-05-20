using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RegvedInventoryDB.Models;
using RegvedInventoryDB.Services;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RegvedInventoryDB.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IDashboardService dashboardService, ILogger<HomeController> logger)
        {
            _dashboardService = dashboardService ?? throw new ArgumentNullException(nameof(dashboardService));
            _logger           = logger           ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var stats = await _dashboardService.GetDashboardStatsAsync();
                return View(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard");
                return View(new DashboardViewModel());
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

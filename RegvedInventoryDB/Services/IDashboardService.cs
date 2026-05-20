using RegvedInventoryDB.Models;
using System.Threading.Tasks;

namespace RegvedInventoryDB.Services
{
    public interface IDashboardService
    {
        Task<DashboardViewModel> GetDashboardStatsAsync();
    }
}

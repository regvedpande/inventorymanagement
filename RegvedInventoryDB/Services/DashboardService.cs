using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RegvedInventoryDB.DAL;
using RegvedInventoryDB.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RegvedInventoryDB.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly InventoryRepository _repository;
        private readonly ILogger<DashboardService> _logger;
        private readonly int _lowStockThreshold;

        public DashboardService(
            InventoryRepository repository,
            ILogger<DashboardService> logger,
            IConfiguration configuration)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _lowStockThreshold = configuration.GetValue<int>("AppSettings:LowStockThreshold", 10);
        }

        public async Task<DashboardViewModel> GetDashboardStatsAsync()
        {
            try
            {
                var result = await _repository.GetDashboardStatsAsync(_lowStockThreshold);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard statistics");
                return new DashboardViewModel();
            }
        }
    }
}

using System.Collections.Generic;

namespace RegvedInventoryDB.Models
{
    public class DashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalVendors { get; set; }
        public int LowStockCount { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public int RecycleBinCount { get; set; }

        public IEnumerable<Product> LowStockProducts { get; set; } = new List<Product>();
        public IEnumerable<Product> RecentProducts { get; set; } = new List<Product>();
    }
}

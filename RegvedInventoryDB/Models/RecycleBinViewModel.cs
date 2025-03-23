using System.Collections.Generic;

namespace RegvedInventoryDB.Models
{
    public class RecycleBinViewModel
    {
        public IEnumerable<Product> SoftDeletedProducts { get; set; } = new List<Product>();
        public IEnumerable<Category> SoftDeletedCategories { get; set; } = new List<Category>();
        public IEnumerable<Vendor> SoftDeletedVendors { get; set; } = new List<Vendor>();
    }
}
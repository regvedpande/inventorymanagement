using RegvedInventoryDB.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RegvedInventoryDB.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task<bool> CreateProductAsync(Product product, int categoryId);
        Task<bool> UpdateProductAsync(Product product, int categoryId);
        Task<bool> DeleteProductAsync(int id, bool permanent);
        Task<List<Product>> GetSoftDeletedProductsAsync();
        Task<bool> RestoreProductAsync(int id);
    }
}
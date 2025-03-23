using Microsoft.Extensions.Logging;
using RegvedInventoryDB.DAL;
using RegvedInventoryDB.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RegvedInventoryDB.Services
{
    public class ProductService : IProductService
    {
        private readonly InventoryRepository _repository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(InventoryRepository repository, ILogger<ProductService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            try
            {
                var response = await _repository.GetProductListAsync();
                return response.Status ? (List<Product>)response.Data : new List<Product>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products");
                return new List<Product>();
            }
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            try
            {
                var response = await _repository.GetProductByIdAsync(id);
                return response.Status ? (Product)response.Data : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product with ID: {Id}", id);
                return null;
            }
        }

        public async Task<bool> CreateProductAsync(Product product, int categoryId)
        {
            try
            {
                _logger.LogInformation("Creating product: {ProductName}", product.ProductName);
                product.CategoryID = categoryId;
                var response = await _repository.InsertProductAsync(product);
                return response.Status;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product: {ProductName}", product.ProductName);
                return false;
            }
        }

        public async Task<bool> UpdateProductAsync(Product product, int categoryId)
        {
            try
            {
                _logger.LogInformation("Updating product: {ProductName}", product.ProductName);
                product.CategoryID = categoryId;
                var response = await _repository.UpdateProductAsync(product);
                return response.Status;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product: {ProductName}", product.ProductName);
                return false;
            }
        }

        public async Task<bool> DeleteProductAsync(int id, bool permanent)
        {
            try
            {
                _logger.LogInformation("Deleting product. ID: {Id}, Permanent: {Permanent}", id, permanent);
                var response = permanent
                    ? await _repository.HardDeleteProductAsync(id)
                    : await _repository.SoftDeleteProductAsync(id);
                return response.Status;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product. ID: {Id}", id);
                return false;
            }
        }

        public async Task<List<Product>> GetSoftDeletedProductsAsync()
        {
            try
            {
                var response = await _repository.GetSoftDeletedProductListAsync();
                return response.Status ? (List<Product>)response.Data : new List<Product>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving soft-deleted products");
                return new List<Product>();
            }
        }

        public async Task<bool> RestoreProductAsync(int id)
        {
            try
            {
                _logger.LogInformation("Restoring product. ID: {Id}", id);
                var response = await _repository.RestoreProductAsync(id);
                return response.Status;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring product. ID: {Id}", id);
                return false;
            }
        }
    }
}
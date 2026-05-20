using Microsoft.Extensions.Logging;
using RegvedInventoryDB.DAL;
using RegvedInventoryDB.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RegvedInventoryDB.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly InventoryRepository _repository;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(InventoryRepository repository, ILogger<CategoryService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            try
            {
                var response = await _repository.GetCategoryListAsync();
                if (!response.Status)
                {
                    _logger.LogWarning("GetCategoryListAsync returned false: {Message}", response.Message);
                    return new List<Category>();
                }
                return (List<Category>)response.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving category list");
                return new List<Category>();
            }
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            try
            {
                var response = await _repository.GetCategoryByIdAsync(id);
                if (!response.Status)
                {
                    _logger.LogWarning("Category not found for ID {Id}: {Message}", id, response.Message);
                    return null;
                }
                return (Category)response.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving category with ID {Id}", id);
                return null;
            }
        }

        public async Task<bool> CreateCategoryAsync(Category category)
        {
            try
            {
                var response = await _repository.InsertCategoryAsync(category);
                if (!response.Status)
                    _logger.LogWarning("Failed to create category '{Name}': {Message}", category.CategoryName, response.Message);
                return response.Status;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category '{Name}'", category.CategoryName);
                return false;
            }
        }

        public async Task<bool> UpdateCategoryAsync(Category category)
        {
            try
            {
                var response = await _repository.UpdateCategoryAsync(category);
                if (!response.Status)
                    _logger.LogWarning("Failed to update category ID {Id}: {Message}", category.CategoryID, response.Message);
                return response.Status;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category ID {Id}", category.CategoryID);
                return false;
            }
        }

        public async Task<bool> DeleteCategoryAsync(int id, bool permanent)
        {
            try
            {
                if (permanent)
                {
                    var products = await GetProductsByCategoryAsync(id);
                    if (products.Count > 0)
                    {
                        _logger.LogWarning("Cannot hard-delete category ID {Id}: {Count} active products exist. Soft-deleting instead.", id, products.Count);
                        var softRes = await _repository.SoftDeleteCategoryAsync(id);
                        return softRes.Status;
                    }

                    var hardRes = await _repository.HardDeleteCategoryAsync(id);
                    if (!hardRes.Status)
                        _logger.LogWarning("Hard delete failed for category ID {Id}: {Message}", id, hardRes.Message);
                    return hardRes.Status;
                }

                var response = await _repository.SoftDeleteCategoryAsync(id);
                if (!response.Status)
                    _logger.LogWarning("Soft delete failed for category ID {Id}: {Message}", id, response.Message);
                return response.Status;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category ID {Id} (permanent={Permanent})", id, permanent);
                return false;
            }
        }

        public async Task<List<Category>> GetSoftDeletedCategoriesAsync()
        {
            try
            {
                var response = await _repository.GetSoftDeletedCategoryListAsync();
                if (!response.Status)
                {
                    _logger.LogWarning("GetSoftDeletedCategoryListAsync returned false: {Message}", response.Message);
                    return new List<Category>();
                }
                return (List<Category>)response.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving soft-deleted categories");
                return new List<Category>();
            }
        }

        public async Task<bool> RestoreCategoryAsync(int id)
        {
            try
            {
                var response = await _repository.RestoreCategoryAsync(id);
                if (!response.Status)
                    _logger.LogWarning("Failed to restore category ID {Id}: {Message}", id, response.Message);
                return response.Status;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring category ID {Id}", id);
                return false;
            }
        }

        private async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            try
            {
                return await _repository.GetProductsByCategoryAsync(categoryId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products for category ID {Id}", categoryId);
                return new List<Product>();
            }
        }
    }
}

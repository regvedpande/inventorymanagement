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

        public CategoryService(InventoryRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            try
            {
                var response = await _repository.GetCategoryListAsync();
                return response.Status ? (List<Category>)response.Data : new List<Category>();
            }
            catch (Exception ex)
            {
                // Log if logger added in future
                return new List<Category>();
            }
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            try
            {
                var response = await _repository.GetCategoryByIdAsync(id);
                return response.Status ? (Category)response.Data : null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> CreateCategoryAsync(Category category)
        {
            try
            {
                var response = await _repository.InsertCategoryAsync(category);
                return response.Status;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UpdateCategoryAsync(Category category)
        {
            try
            {
                var response = await _repository.UpdateCategoryAsync(category);
                return response.Status;
            }
            catch (Exception ex)
            {
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
                    if (products != null && products.Count > 0)
                    {
                        var softDeleteResponse = await _repository.SoftDeleteCategoryAsync(id);
                        return softDeleteResponse.Status;
                    }
                    var hardDeleteResponse = await _repository.HardDeleteCategoryAsync(id);
                    return hardDeleteResponse.Status;
                }
                var response = await _repository.SoftDeleteCategoryAsync(id);
                return response.Status;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<Category>> GetSoftDeletedCategoriesAsync()
        {
            try
            {
                var response = await _repository.GetSoftDeletedCategoryListAsync();
                return response.Status ? (List<Category>)response.Data : new List<Category>();
            }
            catch (Exception ex)
            {
                return new List<Category>();
            }
        }

        public async Task<bool> RestoreCategoryAsync(int id)
        {
            try
            {
                var response = await _repository.RestoreCategoryAsync(id);
                return response.Status;
            }
            catch (Exception ex)
            {
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
                return new List<Product>();
            }
        }
    }
}
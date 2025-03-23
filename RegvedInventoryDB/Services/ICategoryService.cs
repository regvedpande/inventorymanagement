using RegvedInventoryDB.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RegvedInventoryDB.Services
{
    public interface ICategoryService
    {
        Task<List<Category>> GetCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task<bool> CreateCategoryAsync(Category category);
        Task<bool> UpdateCategoryAsync(Category category);
        Task<bool> DeleteCategoryAsync(int id, bool permanent);
        Task<List<Category>> GetSoftDeletedCategoriesAsync();
        Task<bool> RestoreCategoryAsync(int id);
    }
}
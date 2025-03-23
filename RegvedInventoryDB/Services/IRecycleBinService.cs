using RegvedInventoryDB.Models;
using System.Threading.Tasks;

namespace RegvedInventoryDB.Services
{
    public interface IRecycleBinService
    {
        Task<RecycleBinViewModel> GetRecycleBinViewModelAsync();
        Task<bool> RestoreProductAsync(int id);
        Task<bool> PermanentDeleteProductAsync(int id);
        Task<bool> RestoreCategoryAsync(int id);
        Task<bool> PermanentDeleteCategoryAsync(int id);
        Task<bool> RestoreVendorAsync(int id);
        Task<bool> PermanentDeleteVendorAsync(int id);
    }
}
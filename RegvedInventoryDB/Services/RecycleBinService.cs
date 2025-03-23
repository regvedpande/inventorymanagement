using RegvedInventoryDB.DAL;
using RegvedInventoryDB.Models;
using System;
using System.Threading.Tasks;

namespace RegvedInventoryDB.Services
{
    public class RecycleBinService : IRecycleBinService
    {
        private readonly InventoryRepository _repository;

        public RecycleBinService(InventoryRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<RecycleBinViewModel> GetRecycleBinViewModelAsync()
        {
            try
            {
                var prodRes = await _repository.GetSoftDeletedProductListAsync();
                var catRes = await _repository.GetSoftDeletedCategoryListAsync();
                var vendorRes = await _repository.GetSoftDeletedVendorListAsync();

                return new RecycleBinViewModel
                {
                    SoftDeletedProducts = prodRes.Status ? (System.Collections.Generic.IEnumerable<Product>)prodRes.Data : new System.Collections.Generic.List<Product>(),
                    SoftDeletedCategories = catRes.Status ? (System.Collections.Generic.IEnumerable<Category>)catRes.Data : new System.Collections.Generic.List<Category>(),
                    SoftDeletedVendors = vendorRes.Status ? (System.Collections.Generic.IEnumerable<Vendor>)vendorRes.Data : new System.Collections.Generic.List<Vendor>()
                };
            }
            catch (Exception ex)
            {
                // Log if logger added in future
                return new RecycleBinViewModel();
            }
        }

        public async Task<bool> RestoreProductAsync(int id)
        {
            try
            {
                var response = await _repository.RestoreProductAsync(id);
                return response.Status;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> PermanentDeleteProductAsync(int id)
        {
            try
            {
                var response = await _repository.HardDeleteProductAsync(id);
                return response.Status;
            }
            catch (Exception ex)
            {
                return false;
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

        public async Task<bool> PermanentDeleteCategoryAsync(int id)
        {
            try
            {
                var response = await _repository.HardDeleteCategoryAsync(id);
                return response.Status;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> RestoreVendorAsync(int id)
        {
            try
            {
                var response = await _repository.RestoreVendorAsync(id);
                return response.Status;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> PermanentDeleteVendorAsync(int id)
        {
            try
            {
                var response = await _repository.HardDeleteVendorAsync(id);
                return response.Status;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
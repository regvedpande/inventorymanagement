using Microsoft.Extensions.Logging;
using RegvedInventoryDB.DAL;
using RegvedInventoryDB.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RegvedInventoryDB.Services
{
    public class RecycleBinService : IRecycleBinService
    {
        private readonly InventoryRepository _repository;
        private readonly ILogger<RecycleBinService> _logger;

        public RecycleBinService(InventoryRepository repository, ILogger<RecycleBinService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<RecycleBinViewModel> GetRecycleBinViewModelAsync()
        {
            try
            {
                var prodRes   = await _repository.GetSoftDeletedProductListAsync();
                var catRes    = await _repository.GetSoftDeletedCategoryListAsync();
                var vendorRes = await _repository.GetSoftDeletedVendorListAsync();

                return new RecycleBinViewModel
                {
                    SoftDeletedProducts   = prodRes.Status   ? (IEnumerable<Product>)prodRes.Data   : new List<Product>(),
                    SoftDeletedCategories = catRes.Status    ? (IEnumerable<Category>)catRes.Data   : new List<Category>(),
                    SoftDeletedVendors    = vendorRes.Status ? (IEnumerable<Vendor>)vendorRes.Data  : new List<Vendor>()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading recycle bin view model");
                return new RecycleBinViewModel();
            }
        }

        public async Task<bool> RestoreProductAsync(int id)
        {
            try
            {
                var response = await _repository.RestoreProductAsync(id);
                if (!response.Status)
                    _logger.LogWarning("Failed to restore product ID {Id}: {Message}", id, response.Message);
                return response.Status;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring product ID {Id}", id);
                return false;
            }
        }

        public async Task<bool> PermanentDeleteProductAsync(int id)
        {
            try
            {
                var response = await _repository.HardDeleteProductAsync(id);
                if (!response.Status)
                    _logger.LogWarning("Failed to permanently delete product ID {Id}: {Message}", id, response.Message);
                return response.Status;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error permanently deleting product ID {Id}", id);
                return false;
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

        public async Task<bool> PermanentDeleteCategoryAsync(int id)
        {
            try
            {
                var response = await _repository.HardDeleteCategoryAsync(id);
                if (!response.Status)
                    _logger.LogWarning("Failed to permanently delete category ID {Id}: {Message}", id, response.Message);
                return response.Status;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error permanently deleting category ID {Id}", id);
                return false;
            }
        }

        public async Task<bool> RestoreVendorAsync(int id)
        {
            try
            {
                var response = await _repository.RestoreVendorAsync(id);
                if (!response.Status)
                    _logger.LogWarning("Failed to restore vendor ID {Id}: {Message}", id, response.Message);
                return response.Status;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring vendor ID {Id}", id);
                return false;
            }
        }

        public async Task<bool> PermanentDeleteVendorAsync(int id)
        {
            try
            {
                var response = await _repository.HardDeleteVendorAsync(id);
                if (!response.Status)
                    _logger.LogWarning("Failed to permanently delete vendor ID {Id}: {Message}", id, response.Message);
                return response.Status;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error permanently deleting vendor ID {Id}", id);
                return false;
            }
        }
    }
}

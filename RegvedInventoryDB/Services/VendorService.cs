using Microsoft.Extensions.Logging;
using RegvedInventoryDB.DAL;
using RegvedInventoryDB.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RegvedInventoryDB.Services
{
    public class VendorService : IVendorService
    {
        private readonly InventoryRepository _repository;
        private readonly ILogger<VendorService> _logger;

        public VendorService(InventoryRepository repository, ILogger<VendorService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<Vendor>> GetVendorsAsync()
        {
            try
            {
                var response = await _repository.GetVendorListAsync();

                if (!response.Status || !(response.Data is List<Vendor> vendors))
                {
                    _logger.LogError("Vendor retrieval failed. Status: {Status}, Message: {Message}",
                        response.Status, response.Message);
                    return new List<Vendor>();
                }

                _logger.LogInformation("Retrieved {Count} vendors", vendors.Count);
                foreach (var vendor in vendors)
                {
                    _logger.LogDebug("Vendor: {ID} - {Name} (Deleted: {IsDeleted})",
                        vendor.VendorID, vendor.VendorName, vendor.IsDeleted);
                }

                return vendors;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical error retrieving vendors");
                return new List<Vendor>();
            }
        }

        public async Task<Vendor> GetVendorByIdAsync(int id)
        {
            try
            {
                var response = await _repository.GetVendorByIdAsync(id);
                _logger.LogInformation("GetVendorByIdAsync response for ID {Id}: Status={Status}, Data={Data}", id, response.Status, response.Data);
                return response.Status ? (Vendor)response.Data : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving vendor with ID: {Id}", id);
                return null;
            }
        }

        public async Task<bool> CreateVendorAsync(Vendor vendor)
        {
            try
            {
                _logger.LogInformation("Creating vendor: {VendorName}", vendor.VendorName);
                var response = await _repository.InsertVendorAsync(vendor);

                if (!response.Status)
                {
                    _logger.LogError("Failed to save vendor: {VendorName}", vendor.VendorName);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating vendor: {VendorName}", vendor.VendorName);
                return false;
            }
        }

        public async Task<bool> UpdateVendorAsync(Vendor vendor)
        {
            try
            {
                _logger.LogInformation("Updating vendor: {VendorName}", vendor.VendorName);
                var response = await _repository.UpdateVendorAsync(vendor);
                return response.Status;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating vendor: {VendorName}", vendor.VendorName);
                return false;
            }
        }

        public async Task<bool> DeleteVendorAsync(int id, bool permanent)
        {
            try
            {
                _logger.LogInformation("Deleting vendor. ID: {Id}, Permanent: {Permanent}", id, permanent);
                var response = permanent
                    ? await _repository.HardDeleteVendorAsync(id)
                    : await _repository.SoftDeleteVendorAsync(id);
                return response.Status;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting vendor. ID: {Id}", id);
                return false;
            }
        }

        public async Task<List<Vendor>> GetSoftDeletedVendorsAsync()
        {
            try
            {
                var response = await _repository.GetSoftDeletedVendorListAsync();
                return response.Status ? (List<Vendor>)response.Data : new List<Vendor>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving soft-deleted vendors");
                return new List<Vendor>();
            }
        }

        public async Task<bool> RestoreVendorAsync(int id)
        {
            try
            {
                _logger.LogInformation("Restoring vendor. ID: {Id}", id);
                var response = await _repository.RestoreVendorAsync(id);
                return response.Status;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring vendor. ID: {Id}", id);
                return false;
            }
        }
    }
}
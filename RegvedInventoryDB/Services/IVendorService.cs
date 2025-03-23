using RegvedInventoryDB.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RegvedInventoryDB.Services
{
    public interface IVendorService
    {
        Task<List<Vendor>> GetVendorsAsync();
        Task<Vendor> GetVendorByIdAsync(int id);
        Task<bool> CreateVendorAsync(Vendor vendor);
        Task<bool> UpdateVendorAsync(Vendor vendor);
        Task<bool> DeleteVendorAsync(int id, bool permanent);
        Task<List<Vendor>> GetSoftDeletedVendorsAsync();
        Task<bool> RestoreVendorAsync(int id);
    }
}
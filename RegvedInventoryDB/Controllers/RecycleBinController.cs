using Microsoft.AspNetCore.Mvc;
using RegvedInventoryDB.Filters;
using RegvedInventoryDB.Models;
using RegvedInventoryDB.Services;
using System;
using System.Threading.Tasks;

namespace RegvedInventoryDB.Controllers
{
    [Route("RecycleBin")]
    [CustomAuthorizationFilter]
    public class RecycleBinController : Controller
    {
        private readonly IRecycleBinService _recycleBinService;

        public RecycleBinController(IRecycleBinService recycleBinService)
        {
            _recycleBinService = recycleBinService ?? throw new ArgumentNullException(nameof(recycleBinService));
        }

        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var model = await _recycleBinService.GetRecycleBinViewModelAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                // Log error if logger added in future
                return View(new RecycleBinViewModel());
            }
        }

        [HttpPost("RestoreProduct/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreProduct(int id)
        {
            try
            {
                var success = await _recycleBinService.RestoreProductAsync(id);
                return Json(new { success, message = success ? "Product restored successfully." : "Failed to restore product." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error restoring product: {ex.Message}" });
            }
        }

        [HttpPost("PermanentDeleteProduct/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PermanentDeleteProduct(int id)
        {
            try
            {
                var success = await _recycleBinService.PermanentDeleteProductAsync(id);
                return Json(new { success, message = success ? "Product permanently deleted." : "Failed to delete product." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error deleting product: {ex.Message}" });
            }
        }

        [HttpPost("RestoreCategory/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreCategory(int id)
        {
            try
            {
                var success = await _recycleBinService.RestoreCategoryAsync(id);
                return Json(new { success, message = success ? "Category restored successfully." : "Failed to restore category." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error restoring category: {ex.Message}" });
            }
        }

        [HttpPost("PermanentDeleteCategory/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PermanentDeleteCategory(int id)
        {
            try
            {
                var success = await _recycleBinService.PermanentDeleteCategoryAsync(id);
                return Json(new { success, message = success ? "Category permanently deleted." : "Failed to delete category." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error deleting category: {ex.Message}" });
            }
        }

        [HttpPost("RestoreVendor/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreVendor(int id)
        {
            try
            {
                var success = await _recycleBinService.RestoreVendorAsync(id);
                return Json(new { success, message = success ? "Vendor restored successfully." : "Failed to restore vendor." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error restoring vendor: {ex.Message}" });
            }
        }

        [HttpPost("PermanentDeleteVendor/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PermanentDeleteVendor(int id)
        {
            try
            {
                var success = await _recycleBinService.PermanentDeleteVendorAsync(id);
                return Json(new { success, message = success ? "Vendor permanently deleted." : "Failed to delete vendor." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error deleting vendor: {ex.Message}" });
            }
        }
    }
}
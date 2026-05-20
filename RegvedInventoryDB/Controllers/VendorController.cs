using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RegvedInventoryDB.Filters;
using RegvedInventoryDB.Models;
using RegvedInventoryDB.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegvedInventoryDB.Controllers
{
    [Route("Vendor")]
    [CustomAuthorizationFilter]
    public class VendorController : Controller
    {
        private readonly IVendorService _vendorService;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly ILogger<VendorController> _logger;

        public VendorController(
            IVendorService vendorService,
            ICategoryService categoryService,
            IProductService productService,
            ILogger<VendorController> logger)
        {
            _vendorService   = vendorService   ?? throw new ArgumentNullException(nameof(vendorService));
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _productService  = productService  ?? throw new ArgumentNullException(nameof(productService));
            _logger          = logger          ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var vendors = await _vendorService.GetVendorsAsync();
                return View(vendors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving vendor list");
                TempData["Error"] = "Error loading vendor list.";
                return View(new List<Vendor>());
            }
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            try
            {
                var categories = await _categoryService.GetCategoriesAsync();
                var products   = await _productService.GetProductsAsync();

                if (!categories.Any())
                {
                    TempData["Error"] = "Please add at least one category before creating a vendor.";
                    return RedirectToAction(nameof(Index));
                }
                if (!products.Any())
                {
                    TempData["Error"] = "Please add at least one product before creating a vendor.";
                    return RedirectToAction(nameof(Index));
                }

                return View(new VendorCategoryProductViewModel
                {
                    CategoryModel   = categories,
                    ProductModel    = products,
                    VendorModel     = new Vendor(),
                    SelectedCategory = null,
                    SelectedProduct  = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading vendor create form");
                TempData["Error"] = "Error loading the creation form.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VendorCategoryProductViewModel model)
        {
            try
            {
                model.CategoryModel = await _categoryService.GetCategoriesAsync();
                model.ProductModel  = await _productService.GetProductsAsync();

                if (!model.SelectedCategory.HasValue || !model.SelectedProduct.HasValue)
                {
                    ModelState.AddModelError(string.Empty, "Please select both a category and a product.");
                    return View(model);
                }

                if (!ModelState.IsValid)
                    return View(model);

                var vendor = new Vendor
                {
                    VendorName   = model.VendorModel.VendorName,
                    Description  = model.VendorModel.Description,
                    VendorEmail  = model.VendorModel.VendorEmail,
                    Address      = model.VendorModel.Address,
                    PhoneNumber  = model.VendorModel.PhoneNumber,
                    CategoryID   = model.SelectedCategory.Value,
                    ProductID    = model.SelectedProduct.Value,
                    Quantity     = model.VendorModel.Quantity,
                    PricePerUnit = model.VendorModel.PricePerUnit,
                    Amount       = model.VendorModel.Quantity * model.VendorModel.PricePerUnit
                };

                var success = await _vendorService.CreateVendorAsync(vendor);
                if (success)
                {
                    TempData["Success"] = "Vendor created successfully.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = "Failed to save vendor.";
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating vendor");
                TempData["Error"] = "An error occurred while creating the vendor.";
                model.CategoryModel = await _categoryService.GetCategoriesAsync();
                model.ProductModel  = await _productService.GetProductsAsync();
                return View(model);
            }
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var vendor = await _vendorService.GetVendorByIdAsync(id);
                if (vendor == null)
                {
                    _logger.LogWarning("Vendor not found. ID: {Id}", id);
                    return NotFound();
                }
                return View(vendor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading vendor details for ID {Id}", id);
                TempData["Error"] = "Error loading vendor details.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var vendor = await _vendorService.GetVendorByIdAsync(id);
                if (vendor == null)
                {
                    _logger.LogWarning("Vendor not found for edit. ID: {Id}", id);
                    return NotFound();
                }

                return View(new VendorCategoryProductViewModel
                {
                    VendorModel      = vendor,
                    CategoryModel    = await _categoryService.GetCategoriesAsync(),
                    ProductModel     = await _productService.GetProductsAsync(),
                    SelectedCategory = vendor.CategoryID,
                    SelectedProduct  = vendor.ProductID
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading edit form for vendor ID {Id}", id);
                TempData["Error"] = "Error loading edit form.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VendorCategoryProductViewModel model)
        {
            if (id != model.VendorModel.VendorID)
            {
                _logger.LogWarning("Vendor ID mismatch: route={RouteId}, model={ModelId}", id, model.VendorModel.VendorID);
                return BadRequest("ID mismatch.");
            }

            try
            {
                model.CategoryModel = await _categoryService.GetCategoriesAsync();
                model.ProductModel  = await _productService.GetProductsAsync();

                if (!model.SelectedCategory.HasValue || !model.SelectedProduct.HasValue)
                {
                    ModelState.AddModelError(string.Empty, "Please select both a category and a product.");
                    return View(model);
                }

                if (!ModelState.IsValid)
                    return View(model);

                var vendor       = model.VendorModel;
                vendor.CategoryID = model.SelectedCategory.Value;
                vendor.ProductID  = model.SelectedProduct.Value;
                vendor.Amount     = vendor.Quantity * vendor.PricePerUnit;

                var success = await _vendorService.UpdateVendorAsync(vendor);
                if (success)
                {
                    TempData["Success"] = "Vendor updated successfully.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = "Failed to update vendor.";
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating vendor ID {Id}", id);
                TempData["Error"] = "An error occurred while updating the vendor.";
                model.CategoryModel = await _categoryService.GetCategoriesAsync();
                model.ProductModel  = await _productService.GetProductsAsync();
                return View(model);
            }
        }

        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var vendor = await _vendorService.GetVendorByIdAsync(id);
                if (vendor == null)
                {
                    _logger.LogWarning("Vendor not found for delete. ID: {Id}", id);
                    return NotFound();
                }
                return View(vendor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading delete page for vendor ID {Id}", id);
                TempData["Error"] = "Error loading deletion page.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, bool permanent = false)
        {
            try
            {
                var success = await _vendorService.DeleteVendorAsync(id, permanent);
                TempData[success ? "Success" : "Error"] = success
                    ? (permanent ? "Vendor permanently deleted." : "Vendor moved to recycle bin.")
                    : "Deletion failed.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting vendor ID {Id}", id);
                TempData["Error"] = "An error occurred while deleting the vendor.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}

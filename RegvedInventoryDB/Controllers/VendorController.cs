using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RegvedInventoryDB.Models;
using RegvedInventoryDB.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegvedInventoryDB.Controllers
{
    [Route("Vendor")]
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
            _logger = logger;
            _logger.LogInformation("VendorController constructor called");
            _vendorService = vendorService ?? throw new ArgumentNullException(nameof(vendorService));
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Index action called");
            try
            {
                var vendors = await _vendorService.GetVendorsAsync();
                _logger.LogInformation($"Index: Retrieved {vendors.Count} vendors");
                return View(vendors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving vendors");
                TempData["Error"] = "Error loading vendor list";
                return View(new List<Vendor>());
            }
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("Create GET action called");
            try
            {
                var categories = await _categoryService.GetCategoriesAsync();
                var products = await _productService.GetProductsAsync();
                if (!categories.Any() || !products.Any())
                {
                    _logger.LogWarning("No categories or products available");
                    TempData["Error"] = "Please add categories and products first.";
                    return RedirectToAction(nameof(Index));
                }
                return View(new VendorCategoryProductViewModel
                {
                    CategoryModel = categories,
                    ProductModel = products,
                    VendorModel = new Vendor(),
                    SelectedCategory = null,
                    SelectedProduct = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Create form");
                TempData["Error"] = "Error loading the creation form.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VendorCategoryProductViewModel model)
        {
            _logger.LogInformation("Create POST action called");
            try
            {
                model.CategoryModel = await _categoryService.GetCategoriesAsync();
                model.ProductModel = await _productService.GetProductsAsync();
                if (!ModelState.IsValid || !model.SelectedCategory.HasValue || !model.SelectedProduct.HasValue)
                {
                    _logger.LogWarning("Create validation failed");
                    if (!model.SelectedCategory.HasValue || !model.SelectedProduct.HasValue)
                        ModelState.AddModelError("", "Please select a category and product.");
                    return View(model);
                }
                var vendor = new Vendor
                {
                    VendorName = model.VendorModel.VendorName,
                    Description = model.VendorModel.Description,
                    VendorEmail = model.VendorModel.VendorEmail,
                    Address = model.VendorModel.Address,
                    PhoneNumber = model.VendorModel.PhoneNumber,
                    CategoryID = model.SelectedCategory.Value,
                    ProductID = model.SelectedProduct.Value,
                    Quantity = model.VendorModel.Quantity,
                    PricePerUnit = model.VendorModel.PricePerUnit,
                    Amount = model.VendorModel.Quantity * model.VendorModel.PricePerUnit
                };
                if (vendor.Amount <= 0)
                {
                    ModelState.AddModelError("VendorModel.PricePerUnit", "Amount must be greater than 0.");
                    return View(model);
                }
                var success = await _vendorService.CreateVendorAsync(vendor);
                if (!success)
                {
                    TempData["Error"] = "Failed to save vendor.";
                    return View(model);
                }
                TempData["Success"] = "Vendor created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating vendor");
                TempData["Error"] = "An error occurred while creating the vendor.";
                return View(model);
            }
        }

        [HttpGet("Details{id}")]
        public async Task<IActionResult> Details([FromQuery] int id)
        {
            _logger.LogInformation("Details action called for ID: {Id}", id);
            try
            {
                var vendor = await _vendorService.GetVendorByIdAsync(id);
                if (vendor == null)
                {
                    _logger.LogWarning("Vendor not found for ID: {Id}", id);
                    return NotFound();
                }
                return View(vendor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error viewing details for ID: {Id}", id);
                TempData["Error"] = "Error loading vendor details.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet("Edit")]
        public async Task<IActionResult> Edit([FromQuery] int id)
        {
            _logger.LogInformation("Edit GET action called for ID: {Id}", id);
            try
            {
                var vendor = await _vendorService.GetVendorByIdAsync(id);
                if (vendor == null)
                {
                    _logger.LogWarning("Vendor not found for ID: {Id}", id);
                    return NotFound();
                }
                return View(new VendorCategoryProductViewModel
                {
                    VendorModel = vendor,
                    CategoryModel = await _categoryService.GetCategoriesAsync(),
                    ProductModel = await _productService.GetProductsAsync(),
                    SelectedCategory = vendor.CategoryID,
                    SelectedProduct = vendor.ProductID
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Edit form for ID: {Id}", id);
                TempData["Error"] = "Error loading edit form.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromQuery] int id, VendorCategoryProductViewModel model)
        {
            _logger.LogInformation("Edit POST action called for ID: {Id}", id);
            if (id != model.VendorModel.VendorID)
            {
                _logger.LogWarning("ID mismatch: {RouteId} vs {ModelId}", id, model.VendorModel.VendorID);
                return BadRequest("ID mismatch");
            }
            try
            {
                model.CategoryModel = await _categoryService.GetCategoriesAsync();
                model.ProductModel = await _productService.GetProductsAsync();
                if (!ModelState.IsValid || !model.SelectedCategory.HasValue || !model.SelectedProduct.HasValue)
                {
                    _logger.LogWarning("Edit validation failed");
                    if (!model.SelectedCategory.HasValue || !model.SelectedProduct.HasValue)
                        ModelState.AddModelError("", "Please select a category and product.");
                    return View(model);
                }
                var vendor = model.VendorModel;
                vendor.CategoryID = model.SelectedCategory.Value;
                vendor.ProductID = model.SelectedProduct.Value;
                vendor.Amount = vendor.Quantity * vendor.PricePerUnit;
                if (vendor.Amount <= 0)
                {
                    ModelState.AddModelError("VendorModel.PricePerUnit", "Amount must be positive");
                    return View(model);
                }
                var success = await _vendorService.UpdateVendorAsync(vendor);
                if (!success)
                {
                    TempData["Error"] = "Failed to update vendor.";
                    return View(model);
                }
                TempData["Success"] = "Vendor updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating vendor: {Id}", id);
                TempData["Error"] = "Critical error updating vendor.";
                model.CategoryModel = await _categoryService.GetCategoriesAsync();
                model.ProductModel = await _productService.GetProductsAsync();
                return View(model);
            }
        }

        [HttpGet("Delete")]
        public async Task<IActionResult> Delete([FromQuery] int id)
        {
            _logger.LogInformation("Delete GET action called for ID: {Id}", id);
            try
            {
                var vendor = await _vendorService.GetVendorByIdAsync(id);
                if (vendor == null)
                {
                    _logger.LogWarning("Vendor not found for ID: {Id}", id);
                    return NotFound();
                }
                return View(vendor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Delete view for ID: {Id}", id);
                TempData["Error"] = "Error loading deletion page.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromQuery] int id, bool permanent = false)
        {
            _logger.LogInformation("Delete POST action called for ID: {Id}, Permanent: {Permanent}", id, permanent);
            try
            {
                var success = await _vendorService.DeleteVendorAsync(id, permanent);
                TempData[success ? "Success" : "Error"] = success
                    ? $"Vendor {(permanent ? "permanently deleted" : "moved to recycle bin")}"
                    : "Deletion failed";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting vendor: {Id}", id);
                TempData["Error"] = "Critical deletion error.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
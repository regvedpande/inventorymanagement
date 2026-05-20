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
    [Route("Product")]
    [CustomAuthorizationFilter]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(
            IProductService productService,
            ICategoryService categoryService,
            ILogger<ProductController> logger)
        {
            _productService  = productService  ?? throw new ArgumentNullException(nameof(productService));
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _logger          = logger          ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var products = await _productService.GetProductsAsync();
                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products list");
                TempData["Error"] = "An error occurred while loading products.";
                return View(new List<Product>());
            }
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            try
            {
                var categories = await _categoryService.GetCategoriesAsync();
                var viewModel  = new CategoryProductViewModel
                {
                    CategoryModel      = categories ?? new List<Category>(),
                    ProductModelSingle = new Product { ManufactureDate = DateTime.Today }
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error preparing Create product form");
                TempData["Error"] = "An error occurred while loading the form.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryProductViewModel model)
        {
            try
            {
                model.CategoryModel = await _categoryService.GetCategoriesAsync();

                if (!ModelState.IsValid)
                    return View(model);

                var product = new Product
                {
                    ProductName     = model.ProductModelSingle.ProductName,
                    Description     = model.ProductModelSingle.Description,
                    Price           = model.ProductModelSingle.Price,
                    Stock           = model.ProductModelSingle.Stock,
                    ManufactureDate = model.ProductModelSingle.ManufactureDate,
                    CategoryID      = model.CategoryID
                };

                var success = await _productService.CreateProductAsync(product, product.CategoryID);
                if (success)
                {
                    TempData["Success"] = "Product created successfully.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = "Failed to create product. Please try again.";
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                TempData["Error"] = "An error occurred while creating the product.";
                model.CategoryModel = await _categoryService.GetCategoriesAsync();
                return View(model);
            }
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    _logger.LogWarning("Product not found for edit. ID: {Id}", id);
                    return NotFound();
                }

                var viewModel = new CategoryProductViewModel
                {
                    ProductModelSingle = product,
                    CategoryModel      = await _categoryService.GetCategoriesAsync(),
                    CategoryID         = product.CategoryID
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading edit form for product ID {Id}", id);
                TempData["Error"] = "An error occurred while loading the edit form.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryProductViewModel model)
        {
            if (id != model.ProductModelSingle.ProductID)
            {
                _logger.LogWarning("Product ID mismatch: route={RouteId}, model={ModelId}", id, model.ProductModelSingle.ProductID);
                return BadRequest("Product ID mismatch.");
            }

            try
            {
                model.CategoryModel = await _categoryService.GetCategoriesAsync();

                if (!ModelState.IsValid)
                    return View(model);

                var product      = model.ProductModelSingle;
                product.CategoryID = model.CategoryID;

                var success = await _productService.UpdateProductAsync(product, product.CategoryID);
                if (success)
                {
                    TempData["Success"] = "Product updated successfully.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = "Failed to update product.";
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product ID {Id}", id);
                TempData["Error"] = "An error occurred while updating the product.";
                model.CategoryModel = await _categoryService.GetCategoriesAsync();
                return View(model);
            }
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    _logger.LogWarning("Product not found. ID: {Id}", id);
                    return NotFound();
                }
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product details for ID {Id}", id);
                TempData["Error"] = "An error occurred while loading product details.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    _logger.LogWarning("Product not found for delete. ID: {Id}", id);
                    return NotFound();
                }
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading delete page for product ID {Id}", id);
                TempData["Error"] = "An error occurred while loading the delete page.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, bool permanent = false)
        {
            try
            {
                var success = await _productService.DeleteProductAsync(id, permanent);
                TempData[success ? "Success" : "Error"] = success
                    ? (permanent ? "Product permanently deleted." : "Product moved to recycle bin.")
                    : "Failed to delete product.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product ID {Id}", id);
                TempData["Error"] = "An error occurred while deleting the product.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}

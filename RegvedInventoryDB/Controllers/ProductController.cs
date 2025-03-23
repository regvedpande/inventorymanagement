using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RegvedInventoryDB.Filters;
using RegvedInventoryDB.Models;
using RegvedInventoryDB.Services;
using System;
using System.Collections.Generic;
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

        public ProductController(IProductService productService, ICategoryService categoryService, ILogger<ProductController> logger)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
                _logger.LogError(ex, "Error retrieving products for Index");
                TempData["Error"] = "An error occurred while loading products.";
                return View(new List<Product>());
            }
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create(int? id)
        {
            try
            {
                var categories = await _categoryService.GetCategoriesAsync();

                // Add debug logging
                _logger.LogInformation("Categories loaded: {Count}", categories?.Count() ?? 0);
                foreach (var category in categories ?? Enumerable.Empty<Category>())
                {
                    _logger.LogInformation("Category: ID={ID}, Name={Name}", category.CategoryID, category.CategoryName);
                }

                var viewModel = new CategoryProductViewModel
                {
                    CategoryModel = categories ?? new List<Category>(),
                    ProductModelSingle = new Product { ManufactureDate = DateTime.Today },
                    CategoryID = id ?? 0
                };

                // Add debug information to TempData
                TempData["Debug"] = $"Categories loaded: {viewModel.CategoryModel.Count()}";

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
            // Add debug logging
            _logger.LogInformation("Create POST called with CategoryID: {CategoryID}", model.CategoryID);

            try
            {
                // Load categories
                var categories = await _categoryService.GetCategoriesAsync();
                model.CategoryModel = categories;

                // Log ModelState errors
                if (!ModelState.IsValid)
                {
                    foreach (var modelStateEntry in ModelState.Values)
                    {
                        foreach (var error in modelStateEntry.Errors)
                        {
                            _logger.LogWarning("Validation error: {Error}", error.ErrorMessage);
                        }
                    }

                    // Add debug information to ViewData
                    ViewData["Debug"] = $"ModelState errors: {string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))}";
                    return View(model);
                }

                var product = new Product
                {
                    ProductName = model.ProductModelSingle.ProductName,
                    Description = model.ProductModelSingle.Description,
                    Price = model.ProductModelSingle.Price,
                    Stock = model.ProductModelSingle.Stock,
                    ManufactureDate = model.ProductModelSingle.ManufactureDate,
                    CategoryID = model.CategoryID
                };

                _logger.LogInformation("Attempting to create product with CategoryID: {CategoryID}", product.CategoryID);

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
                    _logger.LogWarning("Product not found for edit with ID: {Id}", id);
                    return NotFound();
                }
                var categories = await _categoryService.GetCategoriesAsync();
                var viewModel = new CategoryProductViewModel
                {
                    ProductModelSingle = product,
                    CategoryModel = categories,
                    CategoryID = product.CategoryID
                };
                _logger.LogInformation("Edit GET: Loaded product with CategoryID = {CategoryID}", viewModel.CategoryID);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading edit form for product ID: {Id}", id);
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
                _logger.LogWarning("Product ID mismatch: Route ID {RouteId} vs Model ID {ModelId}", id, model.ProductModelSingle.ProductID);
                return BadRequest("Product ID mismatch.");
            }

            _logger.LogInformation("Product Edit POST action called with CategoryID: {CategoryID}", model.CategoryID);

            try
            {
                model.CategoryModel = await _categoryService.GetCategoriesAsync();

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage);
                    _logger.LogWarning("Validation failed for product edit: {Errors}", string.Join(", ", errors));
                    return View(model);
                }

                var product = model.ProductModelSingle;
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
                _logger.LogError(ex, "Error updating product ID: {Id}", id);
                TempData["Error"] = "An error occurred while updating the product.";
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
                    _logger.LogWarning("Product not found with ID: {Id}", id);
                    return NotFound();
                }
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product details for ID: {Id}", id);
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
                    _logger.LogWarning("Product not found for delete with ID: {Id}", id);
                    return NotFound();
                }
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading delete page for product ID: {Id}", id);
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
                if (success)
                {
                    TempData["Success"] = permanent ? "Product permanently deleted." : "Product moved to recycle bin.";
                }
                else
                {
                    TempData["Error"] = "Failed to delete product.";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product ID: {Id}", id);
                TempData["Error"] = "An error occurred while deleting the product.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
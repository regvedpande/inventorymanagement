using Microsoft.AspNetCore.Mvc;
using RegvedInventoryDB.Filters;
using RegvedInventoryDB.Models;
using RegvedInventoryDB.Services;
using System;
using System.Threading.Tasks;

namespace RegvedInventoryDB.Controllers
{
    [Route("Category")]
    [CustomAuthorizationFilter]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        }

        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetCategoriesAsync();
            return View(categories);
        }

        [HttpGet("RecycleBin")]
        public async Task<IActionResult> RecycleBin()
        {
            var deletedCategories = await _categoryService.GetSoftDeletedCategoriesAsync();
            return View(deletedCategories);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            try
            {
                var success = await _categoryService.CreateCategoryAsync(category);
                if (success)
                {
                    TempData["Success"] = "Category created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                TempData["Error"] = "Failed to create category.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred while creating the category: {ex.Message}";
            }
            return View(category);
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.CategoryID)
            {
                return BadRequest("Category ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return View(category);
            }

            try
            {
                var success = await _categoryService.UpdateCategoryAsync(category);
                if (success)
                {
                    TempData["Success"] = "Category updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                TempData["Error"] = "Failed to update category.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred while updating the category: {ex.Message}";
            }
            return View(category);
        }

        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, bool permanent = false)
        {
            try
            {
                var success = await _categoryService.DeleteCategoryAsync(id, permanent);
                if (success)
                {
                    TempData["Success"] = permanent ? "Category permanently deleted." : "Category moved to recycle bin.";
                }
                else
                {
                    TempData["Error"] = "Failed to delete category.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred while deleting the category: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
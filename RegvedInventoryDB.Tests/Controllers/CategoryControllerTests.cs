using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RegvedInventoryDB.Controllers;
using RegvedInventoryDB.Models;
using RegvedInventoryDB.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace RegvedInventoryDB.Tests.Controllers
{
    public class CategoryControllerTests
    {
        private readonly Mock<ICategoryService> _serviceMock = new();

        [Fact]
        public async Task Index_ReturnsViewResult_WithCategoryList()
        {
            _serviceMock.Setup(s => s.GetCategoriesAsync())
                        .ReturnsAsync(new List<Category>
                        {
                            new() { CategoryID = 1, CategoryName = "Electronics" },
                            new() { CategoryID = 2, CategoryName = "Furniture" }
                        });
            var controller = new CategoryController(_serviceMock.Object);

            var result = await controller.Index();

            var view  = result.Should().BeOfType<ViewResult>().Subject;
            var model = view.Model.Should().BeOfType<List<Category>>().Subject;
            model.Should().HaveCount(2);
        }

        [Fact]
        public async Task Details_WithInvalidId_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.GetCategoryByIdAsync(99))
                        .ReturnsAsync((Category?)null);
            var controller = new CategoryController(_serviceMock.Object);

            var result = await controller.Details(99);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WithValidId_ReturnsViewResult()
        {
            _serviceMock.Setup(s => s.GetCategoryByIdAsync(1))
                        .ReturnsAsync(new Category { CategoryID = 1, CategoryName = "Electronics" });
            var controller = new CategoryController(_serviceMock.Object);

            var result = await controller.Details(1);

            var view  = result.Should().BeOfType<ViewResult>().Subject;
            var model = view.Model.Should().BeOfType<Category>().Subject;
            model.CategoryName.Should().Be("Electronics");
        }

        [Fact]
        public async Task Create_Post_WithValidModel_RedirectsToIndex()
        {
            _serviceMock.Setup(s => s.CreateCategoryAsync(It.IsAny<Category>()))
                        .ReturnsAsync(true);
            var controller = new CategoryController(_serviceMock.Object);
            var category   = new Category { CategoryName = "New Category" };

            var result = await controller.Create(category);

            var redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirect.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task Create_Post_WithInvalidModel_ReturnsView()
        {
            var controller = new CategoryController(_serviceMock.Object);
            controller.ModelState.AddModelError("CategoryName", "Required");

            var result = await controller.Create(new Category());

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Edit_Post_WithIdMismatch_ReturnsBadRequest()
        {
            var controller = new CategoryController(_serviceMock.Object);
            var result     = await controller.Edit(1, new Category { CategoryID = 99 });
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Delete_Post_WithSoftDelete_RedirectsToIndex()
        {
            _serviceMock.Setup(s => s.DeleteCategoryAsync(1, false))
                        .ReturnsAsync(true);
            var controller = new CategoryController(_serviceMock.Object);

            var result = await controller.Delete(1, false);

            var redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirect.ActionName.Should().Be("Index");
        }
    }
}

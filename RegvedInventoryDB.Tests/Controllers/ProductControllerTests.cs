using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RegvedInventoryDB.Controllers;
using RegvedInventoryDB.Models;
using RegvedInventoryDB.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace RegvedInventoryDB.Tests.Controllers
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductService>  _productMock  = new();
        private readonly Mock<ICategoryService> _categoryMock = new();
        private readonly Mock<ILogger<ProductController>> _loggerMock = new();

        private ProductController BuildController()
            => new(_productMock.Object, _categoryMock.Object, _loggerMock.Object);

        [Fact]
        public async Task Index_ReturnsProducts()
        {
            _productMock.Setup(s => s.GetProductsAsync())
                        .ReturnsAsync(new List<Product> { new() { ProductID = 1, ProductName = "Laptop" } });
            var result = await BuildController().Index();

            var view  = result.Should().BeOfType<ViewResult>().Subject;
            var model = view.Model.Should().BeAssignableTo<IEnumerable<Product>>().Subject;
            model.Should().HaveCount(1);
        }

        [Fact]
        public async Task Details_UnknownId_ReturnsNotFound()
        {
            _productMock.Setup(s => s.GetProductByIdAsync(99)).ReturnsAsync((Product?)null);
            var result = await BuildController().Details(99);
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_Get_ReturnsProduct()
        {
            _productMock.Setup(s => s.GetProductByIdAsync(1))
                        .ReturnsAsync(new Product { ProductID = 1, ProductName = "Widget" });
            var result = await BuildController().Delete(1);

            var view  = result.Should().BeOfType<ViewResult>().Subject;
            ((Product)view.Model!).ProductName.Should().Be("Widget");
        }

        [Fact]
        public async Task Delete_Post_Succeeds_SetsSuccessTempData()
        {
            _productMock.Setup(s => s.DeleteProductAsync(1, false)).ReturnsAsync(true);
            var ctrl = BuildController();
            ctrl.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(
                new Microsoft.AspNetCore.Http.DefaultHttpContext(),
                Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());

            var result = await ctrl.Delete(1, false);
            result.Should().BeOfType<RedirectToActionResult>();
        }

        [Fact]
        public void Constructor_ThrowsOnNull()
        {
            var act = () => new ProductController(null!, _categoryMock.Object, _loggerMock.Object);
            act.Should().Throw<ArgumentNullException>();
        }
    }
}

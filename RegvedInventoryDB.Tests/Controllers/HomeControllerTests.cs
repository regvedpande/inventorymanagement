using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RegvedInventoryDB.Controllers;
using RegvedInventoryDB.Models;
using RegvedInventoryDB.Services;
using System.Threading.Tasks;
using Xunit;

namespace RegvedInventoryDB.Tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly Mock<IDashboardService> _dashboardMock  = new();
        private readonly Mock<ILogger<HomeController>> _loggerMock = new();

        [Fact]
        public async Task Index_ReturnsViewResult_WithDashboardViewModel()
        {
            // Arrange
            _dashboardMock.Setup(s => s.GetDashboardStatsAsync())
                          .ReturnsAsync(new DashboardViewModel
                          {
                              TotalProducts   = 5,
                              TotalCategories = 3,
                              TotalVendors    = 2
                          });
            var controller = new HomeController(_dashboardMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.Index();

            // Assert
            var view = result.Should().BeOfType<ViewResult>().Subject;
            var model = view.Model.Should().BeOfType<DashboardViewModel>().Subject;
            model.TotalProducts.Should().Be(5);
            model.TotalCategories.Should().Be(3);
        }

        [Fact]
        public async Task Index_WhenServiceThrows_ReturnsEmptyDashboard()
        {
            // Arrange
            _dashboardMock.Setup(s => s.GetDashboardStatsAsync())
                          .ThrowsAsync(new System.Exception("DB down"));
            var controller = new HomeController(_dashboardMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.Index();

            // Assert
            var view  = result.Should().BeOfType<ViewResult>().Subject;
            var model = view.Model.Should().BeOfType<DashboardViewModel>().Subject;
            model.TotalProducts.Should().Be(0);
        }

        [Fact]
        public void Constructor_ThrowsOnNullDashboardService()
        {
            var act = () => new HomeController(null!, _loggerMock.Object);
            act.Should().Throw<System.ArgumentNullException>()
               .WithMessage("*dashboardService*");
        }

        [Fact]
        public void Constructor_ThrowsOnNullLogger()
        {
            var act = () => new HomeController(_dashboardMock.Object, null!);
            act.Should().Throw<System.ArgumentNullException>()
               .WithMessage("*logger*");
        }
    }
}

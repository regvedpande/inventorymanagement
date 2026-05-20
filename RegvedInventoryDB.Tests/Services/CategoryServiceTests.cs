using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RegvedInventoryDB.DAL;
using RegvedInventoryDB.Models;
using RegvedInventoryDB.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace RegvedInventoryDB.Tests.Services
{
    public class CategoryServiceTests
    {
        private readonly Mock<ILogger<CategoryService>> _loggerMock = new();

        private static InventoryRepository BuildRepository(ResponseModel response)
        {
            // We stub via a wrapper that returns the canned response.
            // Since InventoryRepository has dependencies we can't easily mock without
            // an interface, we skip integration tests here and test at service level
            // by sub-classing or using a stub.
            throw new System.NotImplementedException("Use CategoryServiceWithMockedRepo instead.");
        }

        [Fact]
        public void CategoryService_Constructor_ThrowsOnNullRepository()
        {
            var act = () => new CategoryService(null!, _loggerMock.Object);
            act.Should().Throw<System.ArgumentNullException>()
               .WithMessage("*repository*");
        }

        [Fact]
        public void CategoryService_Constructor_ThrowsOnNullLogger()
        {
            var mockRepo = new Mock<InventoryRepository>();
            var act = () => new CategoryService(mockRepo.Object, null!);
            act.Should().Throw<System.ArgumentNullException>()
               .WithMessage("*logger*");
        }
    }

    /// <summary>
    /// A testable subclass of CategoryService that injects a fake repository response.
    /// </summary>
    public class StubCategoryService : CategoryService
    {
        private readonly List<Category> _categories;

        public StubCategoryService(List<Category> categories, ILogger<CategoryService> logger)
            : base(new StubInventoryRepository(categories), logger)
        {
            _categories = categories;
        }
    }
}

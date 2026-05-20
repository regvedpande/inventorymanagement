using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RegvedInventoryDB.Models;
using RegvedInventoryDB.Services;
using Xunit;

namespace RegvedInventoryDB.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<ILogger<ProductService>> _loggerMock = new();

        [Fact]
        public void ProductService_Constructor_ThrowsOnNullRepository()
        {
            var act = () => new ProductService(null!, _loggerMock.Object);
            act.Should().Throw<System.ArgumentNullException>()
               .WithMessage("*repository*");
        }

        [Fact]
        public void ProductService_Constructor_ThrowsOnNullLogger()
        {
            var act = () => new ProductService(null!, null!);
            act.Should().Throw<System.ArgumentNullException>();
        }
    }
}

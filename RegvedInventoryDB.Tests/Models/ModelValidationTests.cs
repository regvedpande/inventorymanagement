using FluentAssertions;
using RegvedInventoryDB.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace RegvedInventoryDB.Tests.Models
{
    public class ModelValidationTests
    {
        private static IList<ValidationResult> Validate(object model)
        {
            var results = new List<ValidationResult>();
            var ctx     = new ValidationContext(model);
            Validator.TryValidateObject(model, ctx, results, validateAllProperties: true);
            return results;
        }

        // ---- Category ----

        [Fact]
        public void Category_ValidModel_PassesValidation()
        {
            var c = new Category { CategoryName = "Electronics", Description = "Devices" };
            Validate(c).Should().BeEmpty();
        }

        [Fact]
        public void Category_MissingName_FailsValidation()
        {
            var c = new Category { CategoryName = string.Empty };
            var errors = Validate(c);
            errors.Should().ContainSingle(e => e.MemberNames.Contains(nameof(Category.CategoryName)));
        }

        [Fact]
        public void Category_NameTooLong_FailsValidation()
        {
            var c = new Category { CategoryName = new string('A', 101) };
            Validate(c).Should().ContainSingle(e => e.MemberNames.Contains(nameof(Category.CategoryName)));
        }

        // ---- Product ----

        [Fact]
        public void Product_ValidModel_PassesValidation()
        {
            var p = new Product
            {
                ProductName     = "Test Product",
                Price           = 100m,
                Stock           = 10,
                ManufactureDate = DateTime.Today,
                CategoryID      = 1
            };
            Validate(p).Should().BeEmpty();
        }

        [Fact]
        public void Product_NegativePrice_FailsValidation()
        {
            var p = new Product
            {
                ProductName     = "Test",
                Price           = -1m,
                Stock           = 0,
                ManufactureDate = DateTime.Today,
                CategoryID      = 1
            };
            Validate(p).Should().ContainSingle(e => e.MemberNames.Contains(nameof(Product.Price)));
        }

        [Fact]
        public void Product_ZeroPrice_FailsValidation()
        {
            var p = new Product
            {
                ProductName     = "Test",
                Price           = 0m,
                Stock           = 0,
                ManufactureDate = DateTime.Today,
                CategoryID      = 1
            };
            Validate(p).Should().ContainSingle(e => e.MemberNames.Contains(nameof(Product.Price)));
        }

        [Fact]
        public void Product_NegativeStock_FailsValidation()
        {
            var p = new Product
            {
                ProductName     = "Test",
                Price           = 1m,
                Stock           = -1,
                ManufactureDate = DateTime.Today,
                CategoryID      = 1
            };
            Validate(p).Should().ContainSingle(e => e.MemberNames.Contains(nameof(Product.Stock)));
        }

        // ---- Vendor ----

        [Fact]
        public void Vendor_ValidModel_PassesValidation()
        {
            var v = new Vendor
            {
                VendorName   = "ACME Corp",
                VendorEmail  = "info@acme.com",
                Address      = "123 Main St",
                PhoneNumber  = "+1-555-0100",
                CategoryID   = 1,
                ProductID    = 1,
                Quantity     = 10,
                PricePerUnit = 50m
            };
            Validate(v).Should().BeEmpty();
        }

        [Fact]
        public void Vendor_InvalidEmail_FailsValidation()
        {
            var v = new Vendor
            {
                VendorName   = "ACME",
                VendorEmail  = "not-an-email",
                Address      = "123 Main St",
                PhoneNumber  = "+1-555-0100",
                CategoryID   = 1,
                ProductID    = 1,
                Quantity     = 1,
                PricePerUnit = 1m
            };
            Validate(v).Should().ContainSingle(e => e.MemberNames.Contains(nameof(Vendor.VendorEmail)));
        }

        [Fact]
        public void Vendor_ZeroQuantity_FailsValidation()
        {
            var v = new Vendor
            {
                VendorName   = "ACME",
                VendorEmail  = "info@acme.com",
                Address      = "123",
                PhoneNumber  = "+1-555-0100",
                CategoryID   = 1,
                ProductID    = 1,
                Quantity     = 0,
                PricePerUnit = 1m
            };
            Validate(v).Should().ContainSingle(e => e.MemberNames.Contains(nameof(Vendor.Quantity)));
        }

        [Fact]
        public void Vendor_NameTooShort_FailsValidation()
        {
            var v = new Vendor
            {
                VendorName   = "AB",
                VendorEmail  = "x@y.com",
                Address      = "123",
                PhoneNumber  = "123",
                CategoryID   = 1,
                ProductID    = 1,
                Quantity     = 1,
                PricePerUnit = 1m
            };
            Validate(v).Should().ContainSingle(e => e.MemberNames.Contains(nameof(Vendor.VendorName)));
        }

        // ---- DashboardViewModel defaults ----

        [Fact]
        public void DashboardViewModel_DefaultValues_AreZeroAndEmpty()
        {
            var d = new DashboardViewModel();
            d.TotalProducts.Should().Be(0);
            d.TotalCategories.Should().Be(0);
            d.TotalVendors.Should().Be(0);
            d.LowStockCount.Should().Be(0);
            d.TotalInventoryValue.Should().Be(0m);
            d.LowStockProducts.Should().BeEmpty();
            d.RecentProducts.Should().BeEmpty();
        }
    }
}

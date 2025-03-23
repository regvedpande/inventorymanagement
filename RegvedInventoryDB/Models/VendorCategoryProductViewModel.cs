using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RegvedInventoryDB.Models
{
    public class VendorCategoryProductViewModel
    {
        [BindNever]
        public IEnumerable<Category> CategoryModel { get; set; } = new List<Category>();

        [BindNever]
        public IEnumerable<Product> ProductModel { get; set; } = new List<Product>();

        public Vendor VendorModel { get; set; } = new Vendor();

        [Required(ErrorMessage = "Please select a category.")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid category.")]
        public int? SelectedCategory { get; set; }

        [Required(ErrorMessage = "Please select a product.")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid product.")]
        public int? SelectedProduct { get; set; }
    }
}
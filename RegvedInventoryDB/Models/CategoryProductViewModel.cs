using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RegvedInventoryDB.Models
{
    public class CategoryProductViewModel
    {
        public CategoryProductViewModel()
        {
            CategoryModel = new List<Category>();
            ProductModel = new List<Product>();
            ProductModelSingle = new Product();
        }

        public IEnumerable<Category> CategoryModel { get; set; }
        public IEnumerable<Product> ProductModel { get; set; }
        public Product ProductModelSingle { get; set; }

        
        [Required(ErrorMessage = "Category is required")]
        public int CategoryID { get; set; }

        public bool HasCategories => CategoryModel?.Any() == true;
    }
}
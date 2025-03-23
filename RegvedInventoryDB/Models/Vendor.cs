using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RegvedInventoryDB.Models
{
    public class Vendor
    {
        public int VendorID { get; set; }

        [Required(ErrorMessage = "Vendor Name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Vendor Name must be between 3 and 100 characters")]
        public string VendorName { get; set; }

        [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string VendorEmail { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(250, ErrorMessage = "Address cannot exceed 250 characters")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string PhoneNumber { get; set; }

        
        public int CategoryID { get; set; }

        public int ProductID { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Price per unit is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price per unit must be greater than 0")]
        public decimal PricePerUnit { get; set; }

        [BindNever]
        public decimal Amount { get; set; }

        public bool IsDeleted { get; set; } = false; // Added to match DB schema
    }
}
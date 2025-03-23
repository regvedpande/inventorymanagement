using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using RegvedInventoryDB.Models;

namespace RegvedInventoryDB.DAL
{
    public class InventoryRepository
    {
        private readonly string _connectionString;

        public InventoryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("RegvedInventoryDB")
                ?? throw new ArgumentNullException(nameof(configuration), "Connection string 'RegvedInventoryDB' not found.");
        }

        #region Category Methods

        public async Task<ResponseModel> GetCategoryListAsync()
        {
            var res = new ResponseModel();
            var categoryList = new List<Category>();

            try
            {
                using (var con = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_GetCategoryList", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        await con.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                categoryList.Add(new Category
                                {
                                    CategoryID = Convert.ToInt32(reader["CategoryID"]),
                                    CategoryName = reader["CategoryName"].ToString(),
                                    Description = reader["Description"]?.ToString()
                                });
                            }
                        }
                        res.Status = true;
                        res.Data = categoryList;
                        res.Message = "Categories retrieved successfully.";
                    }
                }
            }
            catch (SqlException ex)
            {
                res.Status = false;
                res.Message = $"Database error retrieving categories: {ex.Message} (Error Code: {ex.Number})";
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = $"Unexpected error retrieving categories: {ex.Message}";
            }

            return res;
        }

        public async Task<ResponseModel> GetCategoryByIdAsync(int id)
        {
            var res = new ResponseModel();
            Category category = null;

            try
            {
                using (var con = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_GetCategoryById", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CategoryID", id);
                        await con.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                category = new Category
                                {
                                    CategoryID = Convert.ToInt32(reader["CategoryID"]),
                                    CategoryName = reader["CategoryName"].ToString(),
                                    Description = reader["Description"]?.ToString()
                                };
                            }
                        }
                        res.Status = category != null;
                        res.Data = category;
                        res.Message = category != null ? "Category found." : "Category not found.";
                    }
                }
            }
            catch (SqlException ex)
            {
                res.Status = false;
                res.Message = $"Database error retrieving category ID {id}: {ex.Message}";
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = $"Unexpected error retrieving category ID {id}: {ex.Message}";
            }

            return res;
        }

        public async Task<ResponseModel> InsertCategoryAsync(Category category)
        {
            var res = new ResponseModel();

            try
            {
                using (var con = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_InsertCategory", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CategoryName", category.CategoryName);
                        cmd.Parameters.AddWithValue("@Description", (object)category.Description ?? DBNull.Value);
                        await con.OpenAsync();
                        var result = await cmd.ExecuteScalarAsync();
                        if (result != null && result != DBNull.Value)
                        {
                            res.Status = true;
                            res.Data = Convert.ToInt32(result);
                            res.Message = "Category inserted successfully.";
                        }
                        else
                        {
                            res.Status = false;
                            res.Message = "Failed to insert category.";
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                res.Status = false;
                res.Message = $"Database error inserting category: {ex.Message}";
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = $"Unexpected error inserting category: {ex.Message}";
            }

            return res;
        }

        public async Task<ResponseModel> UpdateCategoryAsync(Category category)
        {
            var res = new ResponseModel();

            try
            {
                using (var con = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_UpdateCategory", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CategoryID", category.CategoryID);
                        cmd.Parameters.AddWithValue("@CategoryName", category.CategoryName);
                        cmd.Parameters.AddWithValue("@Description", (object)category.Description ?? DBNull.Value);
                        await con.OpenAsync();
                        int rows = await cmd.ExecuteNonQueryAsync();
                        res.Status = rows > 0;
                        res.Message = res.Status ? "Category updated successfully." : "Failed to update category.";
                    }
                }
            }
            catch (SqlException ex)
            {
                res.Status = false;
                res.Message = $"Database error updating category: {ex.Message}";
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = $"Unexpected error updating category: {ex.Message}";
            }

            return res;
        }

        public async Task<ResponseModel> SoftDeleteCategoryAsync(int id)
        {
            var res = new ResponseModel();

            try
            {
                using (var con = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_SoftDeleteCategory", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CategoryID", id);
                        await con.OpenAsync();
                        int rows = await cmd.ExecuteNonQueryAsync();
                        res.Status = rows > 0;
                        res.Message = res.Status ? "Category soft deleted successfully." : "Failed to soft delete category.";
                    }
                }
            }
            catch (SqlException ex)
            {
                res.Status = false;
                res.Message = $"Database error soft deleting category ID {id}: {ex.Message}";
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = $"Unexpected error soft deleting category ID {id}: {ex.Message}";
            }

            return res;
        }

        public async Task<ResponseModel> HardDeleteCategoryAsync(int id)
        {
            var res = new ResponseModel();

            try
            {
                using (var con = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_HardDeleteCategory", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CategoryID", id);
                        await con.OpenAsync();
                        int rows = await cmd.ExecuteNonQueryAsync();
                        res.Status = rows > 0;
                        res.Message = res.Status ? "Category hard deleted successfully." : "Failed to hard delete category.";
                    }
                }
            }
            catch (SqlException ex)
            {
                res.Status = false;
                res.Message = $"Database error hard deleting category ID {id}: {ex.Message}";
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = $"Unexpected error hard deleting category ID {id}: {ex.Message}";
            }

            return res;
        }

        public async Task<ResponseModel> GetSoftDeletedCategoryListAsync()
        {
            var res = new ResponseModel();
            var categoryList = new List<Category>();

            try
            {
                using (var con = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_GetSoftDeletedCategories", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        await con.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                categoryList.Add(new Category
                                {
                                    CategoryID = Convert.ToInt32(reader["CategoryID"]),
                                    CategoryName = reader["CategoryName"].ToString(),
                                    Description = reader["Description"]?.ToString()
                                });
                            }
                        }
                        res.Status = true;
                        res.Data = categoryList;
                        res.Message = "Soft-deleted categories retrieved successfully.";
                    }
                }
            }
            catch (SqlException ex)
            {
                res.Status = false;
                res.Message = $"Database error retrieving soft-deleted categories: {ex.Message}";
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = $"Unexpected error retrieving soft-deleted categories: {ex.Message}";
            }

            return res;
        }

        public async Task<ResponseModel> RestoreCategoryAsync(int id)
        {
            var res = new ResponseModel();

            try
            {
                using (var con = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_RestoreCategory", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CategoryID", id);
                        await con.OpenAsync();
                        int rows = await cmd.ExecuteNonQueryAsync();
                        res.Status = rows > 0;
                        res.Message = res.Status ? "Category restored successfully." : "Failed to restore category.";
                    }
                }
            }
            catch (SqlException ex)
            {
                res.Status = false;
                res.Message = $"Database error restoring category ID {id}: {ex.Message}";
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = $"Unexpected error restoring category ID {id}: {ex.Message}";
            }

            return res;
        }

        #endregion

        #region Product Methods

        public async Task<ResponseModel> GetProductListAsync()
        {
            var res = new ResponseModel();
            var productList = new List<Product>();

            try
            {
                using (var con = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_RegvedGetProductList", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        await con.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                productList.Add(new Product
                                {
                                    ProductID = Convert.ToInt32(reader["ProductID"]),
                                    ProductName = reader["ProductName"].ToString(),
                                    Description = reader["Description"]?.ToString(),
                                    Price = Convert.ToDecimal(reader["Price"]),
                                    Stock = Convert.ToInt32(reader["Stock"]),
                                    ManufactureDate = Convert.ToDateTime(reader["ManufactureDate"]),
                                    CategoryName = reader["CategoryName"].ToString()
                                });
                            }
                        }
                        res.Status = true;
                        res.Data = productList;
                        res.Message = "Products retrieved successfully.";
                    }
                }
            }
            catch (SqlException ex)
            {
                res.Status = false;
                res.Message = $"Database error retrieving products: {ex.Message}";
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = $"Unexpected error retrieving products: {ex.Message}";
            }

            return res;
        }

        public async Task<ResponseModel> GetProductByIdAsync(int id)
        {
            var res = new ResponseModel();
            Product product = null;

            try
            {
                using (var con = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_GetProductById", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ProductID", id);
                        await con.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                product = new Product
                                {
                                    ProductID = Convert.ToInt32(reader["ProductID"]),
                                    ProductName = reader["ProductName"].ToString(),
                                    Description = reader["Description"]?.ToString(),
                                    Price = Convert.ToDecimal(reader["Price"]),
                                    Stock = Convert.ToInt32(reader["Stock"]),
                                    ManufactureDate = Convert.ToDateTime(reader["ManufactureDate"]),
                                    CategoryID = Convert.ToInt32(reader["CategoryID"]),
                                    CategoryName = reader["CategoryName"].ToString()
                                };
                            }
                        }
                        res.Status = product != null;
                        res.Data = product;
                        res.Message = product != null ? "Product found." : "Product not found.";
                    }
                }
            }
            catch (SqlException ex)
            {
                res.Status = false;
                res.Message = $"Database error retrieving product ID {id}: {ex.Message}";
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = $"Unexpected error retrieving product ID {id}: {ex.Message}";
            }

            return res;
        }

        public async Task<ResponseModel> InsertProductAsync(Product product)
        {
            var res = new ResponseModel();

            try
            {
                using (var con = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_InsertProduct", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ProductName", product.ProductName);
                        cmd.Parameters.AddWithValue("@Description", (object)product.Description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Price", product.Price);
                        cmd.Parameters.AddWithValue("@Stock", product.Stock);
                        cmd.Parameters.AddWithValue("@ManufactureDate", product.ManufactureDate);
                        cmd.Parameters.AddWithValue("@CategoryID", product.CategoryID);
                        await con.OpenAsync();
                        var result = await cmd.ExecuteScalarAsync();
                        if (result != null && result != DBNull.Value)
                        {
                            res.Status = true;
                            res.Data = Convert.ToInt32(Convert.ToDecimal(result));
                            res.Message = "Product inserted successfully.";
                        }
                        else
                        {
                            res.Status = false;
                            res.Message = "Failed to insert product.";
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                res.Status = false;
                res.Message = $"Database error inserting product: {ex.Message}";
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = $"Unexpected error inserting product: {ex.Message}";
            }

            return res;
        }

        public async Task<ResponseModel> UpdateProductAsync(Product product)
        {
            var res = new ResponseModel();

            try
            {
                using (var con = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_UpdateProduct", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ProductID", product.ProductID);
                        cmd.Parameters.AddWithValue("@ProductName", product.ProductName);
                        cmd.Parameters.AddWithValue("@Description", (object)product.Description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Price", product.Price);
                        cmd.Parameters.AddWithValue("@Stock", product.Stock);
                        cmd.Parameters.AddWithValue("@ManufactureDate", product.ManufactureDate);
                        cmd.Parameters.AddWithValue("@CategoryID", product.CategoryID);
                        await con.OpenAsync();
                        int rows = await cmd.ExecuteNonQueryAsync();
                        res.Status = rows > 0;
                        res.Message = res.Status ? "Product updated successfully." : "Failed to update product.";
                    }
                }
            }
            catch (SqlException ex)
            {
                res.Status = false;
                res.Message = $"Database error updating product: {ex.Message}";
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = $"Unexpected error updating product: {ex.Message}";
            }

            return res;
        }

        public async Task<ResponseModel> SoftDeleteProductAsync(int id)
        {
            var res = new ResponseModel();

            try
            {
                using (var con = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_SoftDeleteProduct", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ProductID", id);
                        await con.OpenAsync();
                        int rows = await cmd.ExecuteNonQueryAsync();
                        res.Status = rows > 0;
                        res.Message = res.Status ? "Product soft deleted successfully." : "Failed to soft delete product.";
                    }
                }
            }
            catch (SqlException ex)
            {
                res.Status = false;
                res.Message = $"Database error soft deleting product ID {id}: {ex.Message}";
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = $"Unexpected error soft deleting product ID {id}: {ex.Message}";
            }

            return res;
        }

        public async Task<ResponseModel> HardDeleteProductAsync(int id)
        {
            var res = new ResponseModel();

            try
            {
                using (var con = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_HardDeleteProduct", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ProductID", id);
                        await con.OpenAsync();
                        int rows = await cmd.ExecuteNonQueryAsync();
                        res.Status = rows > 0;
                        res.Message = res.Status ? "Product hard deleted successfully." : "Failed to hard delete product.";
                    }
                }
            }
            catch (SqlException ex)
            {
                res.Status = false;
                res.Message = $"Database error hard deleting product ID {id}: {ex.Message}";
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = $"Unexpected error hard deleting product ID {id}: {ex.Message}";
            }

            return res;
        }

        public async Task<ResponseModel> GetSoftDeletedProductListAsync()
        {
            var res = new ResponseModel();
            var productList = new List<Product>();

            try
            {
                using (var con = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_GetSoftDeletedProducts", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        await con.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                productList.Add(new Product
                                {
                                    ProductID = Convert.ToInt32(reader["ProductID"]),
                                    ProductName = reader["ProductName"].ToString(),
                                    Description = reader["Description"]?.ToString(),
                                    Price = Convert.ToDecimal(reader["Price"]),
                                    Stock = Convert.ToInt32(reader["Stock"]),
                                    ManufactureDate = Convert.ToDateTime(reader["ManufactureDate"]),
                                    CategoryName = reader["CategoryName"].ToString()
                                });
                            }
                        }
                        res.Status = true;
                        res.Data = productList;
                        res.Message = "Soft-deleted products retrieved successfully.";
                    }
                }
            }
            catch (SqlException ex)
            {
                res.Status = false;
                res.Message = $"Database error retrieving soft-deleted products: {ex.Message}";
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = $"Unexpected error retrieving soft-deleted products: {ex.Message}";
            }

            return res;
        }

        public async Task<ResponseModel> RestoreProductAsync(int id)
        {
            var res = new ResponseModel();

            try
            {
                using (var con = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_RestoreProduct", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ProductID", id);
                        await con.OpenAsync();
                        int rows = await cmd.ExecuteNonQueryAsync();
                        res.Status = rows > 0;
                        res.Message = res.Status ? "Product restored successfully." : "Failed to restore product.";
                    }
                }
            }
            catch (SqlException ex)
            {
                res.Status = false;
                res.Message = $"Database error restoring product ID {id}: {ex.Message}";
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = $"Unexpected error restoring product ID {id}: {ex.Message}";
            }

            return res;
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            var productList = new List<Product>();

            try
            {
                using (var con = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_GetProductsByCategory", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                        await con.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                productList.Add(new Product
                                {
                                    ProductID = Convert.ToInt32(reader["ProductID"]),
                                    ProductName = reader["ProductName"].ToString(),
                                    Description = reader["Description"]?.ToString(),
                                    Price = Convert.ToDecimal(reader["Price"]),
                                    Stock = Convert.ToInt32(reader["Stock"]),
                                    ManufactureDate = Convert.ToDateTime(reader["ManufactureDate"]),
                                    CategoryID = Convert.ToInt32(reader["CategoryID"]),
                                    CategoryName = reader["CategoryName"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Database error retrieving products for category ID {categoryId}: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected error retrieving products for category ID {categoryId}: {ex.Message}", ex);
            }

            return productList;
        }

        #endregion

        #region Vendor Methods

        public async Task<ResponseModel> GetVendorListAsync()
        {
            var res = new ResponseModel();
            var vendorList = new List<Vendor>();

            try
            {
                using (var con = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_GetVendorList", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        await con.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                vendorList.Add(new Vendor
                                {
                                    VendorID = Convert.ToInt32(reader["VendorID"]),
                                    VendorName = reader["VendorName"].ToString(),
                                    Description = reader["Description"]?.ToString(),
                                    VendorEmail = reader["VendorEmail"].ToString(),
                                    Address = reader["Address"].ToString(),
                                    PhoneNumber = reader["PhoneNumber"].ToString(),
                                    CategoryID = Convert.ToInt32(reader["CategoryID"]),
                                    ProductID = Convert.ToInt32(reader["ProductID"]),
                                    Quantity = Convert.ToInt32(reader["Quantity"]),
                                    PricePerUnit = Convert.ToDecimal(reader["PricePerUnit"]),
                                    Amount = Convert.ToDecimal(reader["Amount"]),
                                    IsDeleted = Convert.ToBoolean(reader["IsDeleted"])
                                });
                            }
                        }
                        res.Status = true;
                        res.Data = vendorList;
                        res.Message = "Vendors retrieved successfully.";
                    }
                }
            }
            catch (SqlException ex)
            {
                res.Status = false;
                res.Message = $"Database error retrieving vendors: {ex.Message}";
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = $"Unexpected error retrieving vendors: {ex.Message}";
            }

            return res;
        }

        public async Task<ResponseModel> GetVendorByIdAsync(int id)
        {
            var res = new ResponseModel();
            Vendor vendor = null;

            try
            {
                using (var con = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_GetVendorById", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@VendorID", id);
                        await con.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                vendor = new Vendor
                                {
                                    VendorID = Convert.ToInt32(reader["VendorID"]),
                                    VendorName = reader["VendorName"].ToString(),
                                    Description = reader["Description"]?.ToString(),
                                    VendorEmail = reader["VendorEmail"].ToString(),
                                    Address = reader["Address"].ToString(),
                                    PhoneNumber = reader["PhoneNumber"].ToString(),
                                    CategoryID = Convert.ToInt32(reader["CategoryID"]),
                                    ProductID = Convert.ToInt32(reader["ProductID"]),
                                    Quantity = Convert.ToInt32(reader["Quantity"]),
                                    PricePerUnit = Convert.ToDecimal(reader["PricePerUnit"]),
                                    Amount = Convert.ToDecimal(reader["Amount"]),
                                    IsDeleted = Convert.ToBoolean(reader["IsDeleted"])
                                };
                            }
                        }
                        res.Status = vendor != null;
                        res.Data = vendor;
                        res.Message = vendor != null ? "Vendor found." : "Vendor not found.";
                    }
                }
            }
            catch (SqlException ex)
            {
                res.Status = false;
                res.Message = $"Database error retrieving vendor ID {id}: {ex.Message}";
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = $"Unexpected error retrieving vendor ID {id}: {ex.Message}";
            }

            return res;
        }

        public async Task<ResponseModel> InsertVendorAsync(Vendor vendor)
        {
            var res = new ResponseModel();

            try
            {
                using (var con = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_InsertVendor", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@VendorName", vendor.VendorName);
                        cmd.Parameters.AddWithValue("@Description", (object)vendor.Description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@VendorEmail", vendor.VendorEmail);
                        cmd.Parameters.AddWithValue("@Address", vendor.Address);
                        cmd.Parameters.AddWithValue("@PhoneNumber", vendor.PhoneNumber);
                        cmd.Parameters.AddWithValue("@CategoryID", vendor.CategoryID);
                        cmd.Parameters.AddWithValue("@ProductID", vendor.ProductID);
                        cmd.Parameters.AddWithValue("@Quantity", vendor.Quantity);
                        cmd.Parameters.AddWithValue("@PricePerUnit", vendor.PricePerUnit);
                        cmd.Parameters.AddWithValue("@Amount", vendor.Amount);
                        await con.OpenAsync();
                        var result = await cmd.ExecuteScalarAsync();
                        if (result != null && result != DBNull.Value)
                        {
                            res.Status = true;
                            res.Data = Convert.ToInt32(result);
                            res.Message = "Vendor inserted successfully.";
                        }
                        else
                        {
                            res.Status = false;
                            res.Message = "Failed to insert vendor.";
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                res.Status = false;
                res.Message = $"Database error inserting vendor: {ex.Message}";
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = $"Unexpected error inserting vendor: {ex.Message}";
            }

            return res;
        }

        public async Task<ResponseModel> UpdateVendorAsync(Vendor vendor)
        {
            var res = new ResponseModel();

            try
            {
                using (var con = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_UpdateVendor", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@VendorID", vendor.VendorID);
                        cmd.Parameters.AddWithValue("@VendorName", vendor.VendorName);
                        cmd.Parameters.AddWithValue("@Description", (object)vendor.Description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@VendorEmail", vendor.VendorEmail);
                        cmd.Parameters.AddWithValue("@Address", vendor.Address);
                        cmd.Parameters.AddWithValue("@PhoneNumber", vendor.PhoneNumber);
                        cmd.Parameters.AddWithValue("@CategoryID", vendor.CategoryID);
                        cmd.Parameters.AddWithValue("@ProductID", vendor.ProductID);
                        cmd.Parameters.AddWithValue("@Quantity", vendor.Quantity);
                        cmd.Parameters.AddWithValue("@PricePerUnit", vendor.PricePerUnit);
                        cmd.Parameters.AddWithValue("@Amount", vendor.Amount);
                        await con.OpenAsync();
                        int rows = await cmd.ExecuteNonQueryAsync();
                        res.Status = rows > 0;
                        res.Message = res.Status ? "Vendor updated successfully." : "Failed to update vendor.";
                    }
                }
            }
            catch (SqlException ex)
            {
                res.Status = false;
                res.Message = $"Database error updating vendor: {ex.Message}";
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = $"Unexpected error updating vendor: {ex.Message}";
            }

            return res;
        }

        public async Task<ResponseModel> SoftDeleteVendorAsync(int id)
        {
            var res = new ResponseModel();

            try
            {
                using (var con = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_SoftDeleteVendor", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@VendorID", id);
                        await con.OpenAsync();
                        int rows = await cmd.ExecuteNonQueryAsync();
                        res.Status = rows > 0;
                        res.Message = res.Status ? "Vendor soft deleted successfully." : "Failed to soft delete vendor.";
                    }
                }
            }
            catch (SqlException ex)
            {
                res.Status = false;
                res.Message = $"Database error soft deleting vendor ID {id}: {ex.Message}";
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = $"Unexpected error soft deleting vendor ID {id}: {ex.Message}";
            }

            return res;
        }

        public async Task<ResponseModel> HardDeleteVendorAsync(int id)
        {
            var res = new ResponseModel();

            try
            {
                using (var con = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_HardDeleteVendor", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@VendorID", id);
                        await con.OpenAsync();
                        int rows = await cmd.ExecuteNonQueryAsync();
                        res.Status = rows > 0;
                        res.Message = res.Status ? "Vendor hard deleted successfully." : "Failed to hard delete vendor.";
                    }
                }
            }
            catch (SqlException ex)
            {
                res.Status = false;
                res.Message = $"Database error hard deleting vendor ID {id}: {ex.Message}";
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = $"Unexpected error hard deleting vendor ID {id}: {ex.Message}";
            }

            return res;
        }

        public async Task<ResponseModel> GetSoftDeletedVendorListAsync()
        {
            var res = new ResponseModel();
            var vendorList = new List<Vendor>();

            try
            {
                using (var con = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_GetSoftDeletedVendors", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        await con.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                vendorList.Add(new Vendor
                                {
                                    VendorID = Convert.ToInt32(reader["VendorID"]),
                                    VendorName = reader["VendorName"].ToString(),
                                    Description = reader["Description"]?.ToString(),
                                    VendorEmail = reader["VendorEmail"].ToString(),
                                    Address = reader["Address"].ToString(),
                                    PhoneNumber = reader["PhoneNumber"].ToString(),
                                    CategoryID = Convert.ToInt32(reader["CategoryID"]),
                                    ProductID = Convert.ToInt32(reader["ProductID"]),
                                    Quantity = Convert.ToInt32(reader["Quantity"]),
                                    PricePerUnit = Convert.ToDecimal(reader["PricePerUnit"]),
                                    Amount = Convert.ToDecimal(reader["Amount"]),
                                    IsDeleted = Convert.ToBoolean(reader["IsDeleted"])
                                });
                            }
                        }
                        res.Status = true;
                        res.Data = vendorList;
                        res.Message = "Soft-deleted vendors retrieved successfully.";
                    }
                }
            }
            catch (SqlException ex)
            {
                res.Status = false;
                res.Message = $"Database error retrieving soft-deleted vendors: {ex.Message}";
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = $"Unexpected error retrieving soft-deleted vendors: {ex.Message}";
            }

            return res;
        }

        public async Task<ResponseModel> RestoreVendorAsync(int id)
        {
            var res = new ResponseModel();

            try
            {
                using (var con = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_RestoreVendor", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@VendorID", id);
                        await con.OpenAsync();
                        int rows = await cmd.ExecuteNonQueryAsync();
                        res.Status = rows > 0;
                        res.Message = res.Status ? "Vendor restored successfully." : "Failed to restore vendor.";
                    }
                }
            }
            catch (SqlException ex)
            {
                res.Status = false;
                res.Message = $"Database error restoring vendor ID {id}: {ex.Message}";
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = $"Unexpected error restoring vendor ID {id}: {ex.Message}";
            }

            return res;
        }

        #endregion
    }
}
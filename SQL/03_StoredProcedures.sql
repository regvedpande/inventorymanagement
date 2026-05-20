-- =============================================================
-- Script: 03_StoredProcedures.sql
-- Description: Creates all stored procedures for the Inventory
--              Management System
-- =============================================================

USE RegvedInventoryDB;
GO

-- ===========================================
-- CATEGORY STORED PROCEDURES
-- ===========================================

-- sp_GetCategoryList
IF OBJECT_ID('sp_GetCategoryList', 'P') IS NOT NULL DROP PROCEDURE sp_GetCategoryList;
GO
CREATE PROCEDURE sp_GetCategoryList
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CategoryID, CategoryName, Description, IsDeleted
    FROM   Categories
    WHERE  IsDeleted = 0
    ORDER  BY CategoryName;
END;
GO

-- sp_GetCategoryById
IF OBJECT_ID('sp_GetCategoryById', 'P') IS NOT NULL DROP PROCEDURE sp_GetCategoryById;
GO
CREATE PROCEDURE sp_GetCategoryById
    @CategoryID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CategoryID, CategoryName, Description, IsDeleted
    FROM   Categories
    WHERE  CategoryID = @CategoryID AND IsDeleted = 0;
END;
GO

-- sp_InsertCategory
IF OBJECT_ID('sp_InsertCategory', 'P') IS NOT NULL DROP PROCEDURE sp_InsertCategory;
GO
CREATE PROCEDURE sp_InsertCategory
    @CategoryName NVARCHAR(100),
    @Description  NVARCHAR(250) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Categories (CategoryName, Description)
    VALUES (@CategoryName, @Description);
    SELECT SCOPE_IDENTITY() AS NewCategoryID;
END;
GO

-- sp_UpdateCategory
IF OBJECT_ID('sp_UpdateCategory', 'P') IS NOT NULL DROP PROCEDURE sp_UpdateCategory;
GO
CREATE PROCEDURE sp_UpdateCategory
    @CategoryID   INT,
    @CategoryName NVARCHAR(100),
    @Description  NVARCHAR(250) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Categories
    SET    CategoryName = @CategoryName,
           Description  = @Description,
           UpdatedAt    = GETUTCDATE()
    WHERE  CategoryID = @CategoryID AND IsDeleted = 0;
END;
GO

-- sp_SoftDeleteCategory
IF OBJECT_ID('sp_SoftDeleteCategory', 'P') IS NOT NULL DROP PROCEDURE sp_SoftDeleteCategory;
GO
CREATE PROCEDURE sp_SoftDeleteCategory
    @CategoryID INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Categories
    SET    IsDeleted = 1, UpdatedAt = GETUTCDATE()
    WHERE  CategoryID = @CategoryID AND IsDeleted = 0;
END;
GO

-- sp_HardDeleteCategory
IF OBJECT_ID('sp_HardDeleteCategory', 'P') IS NOT NULL DROP PROCEDURE sp_HardDeleteCategory;
GO
CREATE PROCEDURE sp_HardDeleteCategory
    @CategoryID INT
AS
BEGIN
    SET NOCOUNT ON;
    -- Prevent hard delete if active products reference this category
    IF EXISTS (SELECT 1 FROM Products WHERE CategoryID = @CategoryID AND IsDeleted = 0)
    BEGIN
        RAISERROR('Cannot permanently delete a category that has active products.', 16, 1);
        RETURN;
    END;
    DELETE FROM Categories WHERE CategoryID = @CategoryID;
END;
GO

-- sp_GetSoftDeletedCategories
IF OBJECT_ID('sp_GetSoftDeletedCategories', 'P') IS NOT NULL DROP PROCEDURE sp_GetSoftDeletedCategories;
GO
CREATE PROCEDURE sp_GetSoftDeletedCategories
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CategoryID, CategoryName, Description, IsDeleted
    FROM   Categories
    WHERE  IsDeleted = 1
    ORDER  BY CategoryName;
END;
GO

-- sp_RestoreCategory
IF OBJECT_ID('sp_RestoreCategory', 'P') IS NOT NULL DROP PROCEDURE sp_RestoreCategory;
GO
CREATE PROCEDURE sp_RestoreCategory
    @CategoryID INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Categories
    SET    IsDeleted = 0, UpdatedAt = GETUTCDATE()
    WHERE  CategoryID = @CategoryID AND IsDeleted = 1;
END;
GO

-- ===========================================
-- PRODUCT STORED PROCEDURES
-- ===========================================

-- sp_RegvedGetProductList
IF OBJECT_ID('sp_RegvedGetProductList', 'P') IS NOT NULL DROP PROCEDURE sp_RegvedGetProductList;
GO
CREATE PROCEDURE sp_RegvedGetProductList
AS
BEGIN
    SET NOCOUNT ON;
    SELECT p.ProductID, p.ProductName, p.Description, p.Price, p.Stock,
           p.ManufactureDate, p.CategoryID, c.CategoryName, p.IsDeleted
    FROM   Products p
    INNER  JOIN Categories c ON p.CategoryID = c.CategoryID
    WHERE  p.IsDeleted = 0
    ORDER  BY p.ProductName;
END;
GO

-- sp_GetProductById
IF OBJECT_ID('sp_GetProductById', 'P') IS NOT NULL DROP PROCEDURE sp_GetProductById;
GO
CREATE PROCEDURE sp_GetProductById
    @ProductID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT p.ProductID, p.ProductName, p.Description, p.Price, p.Stock,
           p.ManufactureDate, p.CategoryID, c.CategoryName, p.IsDeleted
    FROM   Products p
    INNER  JOIN Categories c ON p.CategoryID = c.CategoryID
    WHERE  p.ProductID = @ProductID AND p.IsDeleted = 0;
END;
GO

-- sp_InsertProduct
IF OBJECT_ID('sp_InsertProduct', 'P') IS NOT NULL DROP PROCEDURE sp_InsertProduct;
GO
CREATE PROCEDURE sp_InsertProduct
    @ProductName     NVARCHAR(100),
    @Description     NVARCHAR(250) = NULL,
    @Price           DECIMAL(18,2),
    @Stock           INT,
    @ManufactureDate DATE,
    @CategoryID      INT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Products (ProductName, Description, Price, Stock, ManufactureDate, CategoryID)
    VALUES (@ProductName, @Description, @Price, @Stock, @ManufactureDate, @CategoryID);
    SELECT SCOPE_IDENTITY() AS NewProductID;
END;
GO

-- sp_UpdateProduct
IF OBJECT_ID('sp_UpdateProduct', 'P') IS NOT NULL DROP PROCEDURE sp_UpdateProduct;
GO
CREATE PROCEDURE sp_UpdateProduct
    @ProductID       INT,
    @ProductName     NVARCHAR(100),
    @Description     NVARCHAR(250) = NULL,
    @Price           DECIMAL(18,2),
    @Stock           INT,
    @ManufactureDate DATE,
    @CategoryID      INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Products
    SET    ProductName     = @ProductName,
           Description     = @Description,
           Price           = @Price,
           Stock           = @Stock,
           ManufactureDate = @ManufactureDate,
           CategoryID      = @CategoryID,
           UpdatedAt       = GETUTCDATE()
    WHERE  ProductID = @ProductID AND IsDeleted = 0;
END;
GO

-- sp_SoftDeleteProduct
IF OBJECT_ID('sp_SoftDeleteProduct', 'P') IS NOT NULL DROP PROCEDURE sp_SoftDeleteProduct;
GO
CREATE PROCEDURE sp_SoftDeleteProduct
    @ProductID INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Products
    SET    IsDeleted = 1, UpdatedAt = GETUTCDATE()
    WHERE  ProductID = @ProductID AND IsDeleted = 0;
END;
GO

-- sp_HardDeleteProduct
IF OBJECT_ID('sp_HardDeleteProduct', 'P') IS NOT NULL DROP PROCEDURE sp_HardDeleteProduct;
GO
CREATE PROCEDURE sp_HardDeleteProduct
    @ProductID INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Products WHERE ProductID = @ProductID;
END;
GO

-- sp_GetSoftDeletedProducts
IF OBJECT_ID('sp_GetSoftDeletedProducts', 'P') IS NOT NULL DROP PROCEDURE sp_GetSoftDeletedProducts;
GO
CREATE PROCEDURE sp_GetSoftDeletedProducts
AS
BEGIN
    SET NOCOUNT ON;
    SELECT p.ProductID, p.ProductName, p.Description, p.Price, p.Stock,
           p.ManufactureDate, p.CategoryID, c.CategoryName, p.IsDeleted
    FROM   Products p
    INNER  JOIN Categories c ON p.CategoryID = c.CategoryID
    WHERE  p.IsDeleted = 1
    ORDER  BY p.ProductName;
END;
GO

-- sp_RestoreProduct
IF OBJECT_ID('sp_RestoreProduct', 'P') IS NOT NULL DROP PROCEDURE sp_RestoreProduct;
GO
CREATE PROCEDURE sp_RestoreProduct
    @ProductID INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Products
    SET    IsDeleted = 0, UpdatedAt = GETUTCDATE()
    WHERE  ProductID = @ProductID AND IsDeleted = 1;
END;
GO

-- sp_GetProductsByCategory
IF OBJECT_ID('sp_GetProductsByCategory', 'P') IS NOT NULL DROP PROCEDURE sp_GetProductsByCategory;
GO
CREATE PROCEDURE sp_GetProductsByCategory
    @CategoryID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT p.ProductID, p.ProductName, p.Description, p.Price, p.Stock,
           p.ManufactureDate, p.CategoryID, c.CategoryName, p.IsDeleted
    FROM   Products p
    INNER  JOIN Categories c ON p.CategoryID = c.CategoryID
    WHERE  p.CategoryID = @CategoryID AND p.IsDeleted = 0
    ORDER  BY p.ProductName;
END;
GO

-- ===========================================
-- VENDOR STORED PROCEDURES
-- ===========================================

-- sp_GetVendorList
IF OBJECT_ID('sp_GetVendorList', 'P') IS NOT NULL DROP PROCEDURE sp_GetVendorList;
GO
CREATE PROCEDURE sp_GetVendorList
AS
BEGIN
    SET NOCOUNT ON;
    SELECT v.VendorID, v.VendorName, v.Description, v.VendorEmail,
           v.Address, v.PhoneNumber, v.CategoryID, v.ProductID,
           v.Quantity, v.PricePerUnit, v.Amount, v.IsDeleted,
           c.CategoryName, p.ProductName
    FROM   Vendors v
    INNER  JOIN Categories c ON v.CategoryID = c.CategoryID
    INNER  JOIN Products   p ON v.ProductID  = p.ProductID
    WHERE  v.IsDeleted = 0
    ORDER  BY v.VendorName;
END;
GO

-- sp_GetVendorById
IF OBJECT_ID('sp_GetVendorById', 'P') IS NOT NULL DROP PROCEDURE sp_GetVendorById;
GO
CREATE PROCEDURE sp_GetVendorById
    @VendorID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT v.VendorID, v.VendorName, v.Description, v.VendorEmail,
           v.Address, v.PhoneNumber, v.CategoryID, v.ProductID,
           v.Quantity, v.PricePerUnit, v.Amount, v.IsDeleted,
           c.CategoryName, p.ProductName
    FROM   Vendors v
    INNER  JOIN Categories c ON v.CategoryID = c.CategoryID
    INNER  JOIN Products   p ON v.ProductID  = p.ProductID
    WHERE  v.VendorID = @VendorID AND v.IsDeleted = 0;
END;
GO

-- sp_InsertVendor
IF OBJECT_ID('sp_InsertVendor', 'P') IS NOT NULL DROP PROCEDURE sp_InsertVendor;
GO
CREATE PROCEDURE sp_InsertVendor
    @VendorName   NVARCHAR(100),
    @Description  NVARCHAR(250) = NULL,
    @VendorEmail  NVARCHAR(100),
    @Address      NVARCHAR(250),
    @PhoneNumber  NVARCHAR(20),
    @CategoryID   INT,
    @ProductID    INT,
    @Quantity     INT,
    @PricePerUnit DECIMAL(18,2),
    @Amount       DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Vendors (VendorName, Description, VendorEmail, Address, PhoneNumber,
                         CategoryID, ProductID, Quantity, PricePerUnit)
    VALUES (@VendorName, @Description, @VendorEmail, @Address, @PhoneNumber,
            @CategoryID, @ProductID, @Quantity, @PricePerUnit);
    SELECT SCOPE_IDENTITY() AS NewVendorID;
END;
GO

-- sp_UpdateVendor
IF OBJECT_ID('sp_UpdateVendor', 'P') IS NOT NULL DROP PROCEDURE sp_UpdateVendor;
GO
CREATE PROCEDURE sp_UpdateVendor
    @VendorID     INT,
    @VendorName   NVARCHAR(100),
    @Description  NVARCHAR(250) = NULL,
    @VendorEmail  NVARCHAR(100),
    @Address      NVARCHAR(250),
    @PhoneNumber  NVARCHAR(20),
    @CategoryID   INT,
    @ProductID    INT,
    @Quantity     INT,
    @PricePerUnit DECIMAL(18,2),
    @Amount       DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Vendors
    SET    VendorName   = @VendorName,
           Description  = @Description,
           VendorEmail  = @VendorEmail,
           Address      = @Address,
           PhoneNumber  = @PhoneNumber,
           CategoryID   = @CategoryID,
           ProductID    = @ProductID,
           Quantity     = @Quantity,
           PricePerUnit = @PricePerUnit,
           UpdatedAt    = GETUTCDATE()
    WHERE  VendorID = @VendorID AND IsDeleted = 0;
END;
GO

-- sp_SoftDeleteVendor
IF OBJECT_ID('sp_SoftDeleteVendor', 'P') IS NOT NULL DROP PROCEDURE sp_SoftDeleteVendor;
GO
CREATE PROCEDURE sp_SoftDeleteVendor
    @VendorID INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Vendors
    SET    IsDeleted = 1, UpdatedAt = GETUTCDATE()
    WHERE  VendorID = @VendorID AND IsDeleted = 0;
END;
GO

-- sp_HardDeleteVendor
IF OBJECT_ID('sp_HardDeleteVendor', 'P') IS NOT NULL DROP PROCEDURE sp_HardDeleteVendor;
GO
CREATE PROCEDURE sp_HardDeleteVendor
    @VendorID INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Vendors WHERE VendorID = @VendorID;
END;
GO

-- sp_GetSoftDeletedVendors
IF OBJECT_ID('sp_GetSoftDeletedVendors', 'P') IS NOT NULL DROP PROCEDURE sp_GetSoftDeletedVendors;
GO
CREATE PROCEDURE sp_GetSoftDeletedVendors
AS
BEGIN
    SET NOCOUNT ON;
    SELECT v.VendorID, v.VendorName, v.Description, v.VendorEmail,
           v.Address, v.PhoneNumber, v.CategoryID, v.ProductID,
           v.Quantity, v.PricePerUnit, v.Amount, v.IsDeleted,
           c.CategoryName, p.ProductName
    FROM   Vendors v
    INNER  JOIN Categories c ON v.CategoryID = c.CategoryID
    INNER  JOIN Products   p ON v.ProductID  = p.ProductID
    WHERE  v.IsDeleted = 1
    ORDER  BY v.VendorName;
END;
GO

-- sp_RestoreVendor
IF OBJECT_ID('sp_RestoreVendor', 'P') IS NOT NULL DROP PROCEDURE sp_RestoreVendor;
GO
CREATE PROCEDURE sp_RestoreVendor
    @VendorID INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Vendors
    SET    IsDeleted = 0, UpdatedAt = GETUTCDATE()
    WHERE  VendorID = @VendorID AND IsDeleted = 1;
END;
GO

-- ===========================================
-- DASHBOARD STORED PROCEDURE
-- ===========================================

IF OBJECT_ID('sp_GetDashboardStats', 'P') IS NOT NULL DROP PROCEDURE sp_GetDashboardStats;
GO
CREATE PROCEDURE sp_GetDashboardStats
    @LowStockThreshold INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        (SELECT COUNT(*) FROM Products   WHERE IsDeleted = 0)                             AS TotalProducts,
        (SELECT COUNT(*) FROM Categories WHERE IsDeleted = 0)                             AS TotalCategories,
        (SELECT COUNT(*) FROM Vendors    WHERE IsDeleted = 0)                             AS TotalVendors,
        (SELECT COUNT(*) FROM Products   WHERE IsDeleted = 0 AND Stock <= @LowStockThreshold) AS LowStockCount,
        (SELECT ISNULL(SUM(CAST(Price AS DECIMAL(18,2)) * Stock), 0)
         FROM Products WHERE IsDeleted = 0)                                                AS TotalInventoryValue,
        (SELECT COUNT(*) FROM Products   WHERE IsDeleted = 1)                             AS DeletedProductsCount,
        (SELECT COUNT(*) FROM Categories WHERE IsDeleted = 1)                             AS DeletedCategoriesCount,
        (SELECT COUNT(*) FROM Vendors    WHERE IsDeleted = 1)                             AS DeletedVendorsCount;

    -- Low stock products list
    SELECT TOP 10 p.ProductID, p.ProductName, p.Description, p.Price, p.Stock,
           p.ManufactureDate, p.CategoryID, c.CategoryName, p.IsDeleted
    FROM   Products p
    INNER  JOIN Categories c ON p.CategoryID = c.CategoryID
    WHERE  p.IsDeleted = 0 AND p.Stock <= @LowStockThreshold
    ORDER  BY p.Stock ASC;

    -- Recent 5 products
    SELECT TOP 5 p.ProductID, p.ProductName, p.Description, p.Price, p.Stock,
           p.ManufactureDate, p.CategoryID, c.CategoryName, p.IsDeleted
    FROM   Products p
    INNER  JOIN Categories c ON p.CategoryID = c.CategoryID
    WHERE  p.IsDeleted = 0
    ORDER  BY p.CreatedAt DESC;
END;
GO

PRINT 'All stored procedures created successfully.';
GO

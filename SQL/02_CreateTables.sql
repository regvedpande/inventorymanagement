-- =============================================================
-- Script: 02_CreateTables.sql
-- Description: Creates all tables for the Inventory Management System
-- =============================================================

USE RegvedInventoryDB;
GO

-- -------------------------
-- Table: Categories
-- -------------------------
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Categories')
BEGIN
    CREATE TABLE Categories (
        CategoryID    INT IDENTITY(1,1) PRIMARY KEY,
        CategoryName  NVARCHAR(100) NOT NULL,
        Description   NVARCHAR(250) NULL,
        IsDeleted     BIT NOT NULL DEFAULT 0,
        CreatedAt     DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt     DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
    PRINT 'Table Categories created.';
END
ELSE
    PRINT 'Table Categories already exists.';
GO

-- -------------------------
-- Table: Products
-- -------------------------
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Products')
BEGIN
    CREATE TABLE Products (
        ProductID       INT IDENTITY(1,1) PRIMARY KEY,
        ProductName     NVARCHAR(100) NOT NULL,
        Description     NVARCHAR(250) NULL,
        Price           DECIMAL(18,2) NOT NULL CHECK (Price > 0),
        Stock           INT NOT NULL DEFAULT 0 CHECK (Stock >= 0),
        ManufactureDate DATE NOT NULL,
        CategoryID      INT NOT NULL,
        IsDeleted       BIT NOT NULL DEFAULT 0,
        CreatedAt       DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt       DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryID)
            REFERENCES Categories(CategoryID)
    );
    PRINT 'Table Products created.';
END
ELSE
    PRINT 'Table Products already exists.';
GO

-- -------------------------
-- Table: Vendors
-- -------------------------
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Vendors')
BEGIN
    CREATE TABLE Vendors (
        VendorID      INT IDENTITY(1,1) PRIMARY KEY,
        VendorName    NVARCHAR(100) NOT NULL,
        Description   NVARCHAR(250) NULL,
        VendorEmail   NVARCHAR(100) NOT NULL,
        Address       NVARCHAR(250) NOT NULL,
        PhoneNumber   NVARCHAR(20) NOT NULL,
        CategoryID    INT NOT NULL,
        ProductID     INT NOT NULL,
        Quantity      INT NOT NULL CHECK (Quantity > 0),
        PricePerUnit  DECIMAL(18,2) NOT NULL CHECK (PricePerUnit > 0),
        Amount        AS (CAST(Quantity AS DECIMAL(18,2)) * PricePerUnit) PERSISTED,
        IsDeleted     BIT NOT NULL DEFAULT 0,
        CreatedAt     DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt     DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_Vendors_Categories FOREIGN KEY (CategoryID)
            REFERENCES Categories(CategoryID),
        CONSTRAINT FK_Vendors_Products FOREIGN KEY (ProductID)
            REFERENCES Products(ProductID)
    );
    PRINT 'Table Vendors created.';
END
ELSE
    PRINT 'Table Vendors already exists.';
GO

-- -------------------------
-- Indexes for performance
-- -------------------------
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Products_CategoryID' AND object_id = OBJECT_ID('Products'))
    CREATE INDEX IX_Products_CategoryID ON Products(CategoryID);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Products_IsDeleted' AND object_id = OBJECT_ID('Products'))
    CREATE INDEX IX_Products_IsDeleted ON Products(IsDeleted);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Categories_IsDeleted' AND object_id = OBJECT_ID('Categories'))
    CREATE INDEX IX_Categories_IsDeleted ON Categories(IsDeleted);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Vendors_IsDeleted' AND object_id = OBJECT_ID('Vendors'))
    CREATE INDEX IX_Vendors_IsDeleted ON Vendors(IsDeleted);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Vendors_CategoryID' AND object_id = OBJECT_ID('Vendors'))
    CREATE INDEX IX_Vendors_CategoryID ON Vendors(CategoryID);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Vendors_ProductID' AND object_id = OBJECT_ID('Vendors'))
    CREATE INDEX IX_Vendors_ProductID ON Vendors(ProductID);

PRINT 'Indexes created successfully.';
GO

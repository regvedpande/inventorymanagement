-- =============================================================
-- Script: 04_SeedData.sql
-- Description: Inserts sample seed data for development/testing
-- =============================================================

USE RegvedInventoryDB;
GO

-- Seed Categories
IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryName = 'Electronics')
BEGIN
    INSERT INTO Categories (CategoryName, Description)
    VALUES
        ('Electronics',   'Electronic devices and accessories'),
        ('Furniture',      'Office and home furniture'),
        ('Stationery',     'Office and school supplies'),
        ('Clothing',       'Apparel and accessories'),
        ('Food & Beverage','Food items and drinks');
    PRINT 'Seed categories inserted.';
END
GO

-- Seed Products
IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductName = 'Laptop Pro X')
BEGIN
    INSERT INTO Products (ProductName, Description, Price, Stock, ManufactureDate, CategoryID)
    VALUES
        ('Laptop Pro X',      'High-performance laptop',           75000.00,  15, '2024-01-15', 1),
        ('Wireless Mouse',    'Ergonomic wireless mouse',           1200.00,   50, '2024-03-01', 1),
        ('USB-C Hub',         '7-in-1 USB-C Hub',                  2500.00,   30, '2024-02-10', 1),
        ('Office Chair',      'Ergonomic office chair',            12000.00,   8, '2023-11-05', 2),
        ('Standing Desk',     'Height-adjustable standing desk',   25000.00,   5, '2023-12-01', 2),
        ('Ballpoint Pens',    'Box of 12 ballpoint pens',            150.00, 200, '2024-04-01', 3),
        ('A4 Paper Ream',     '500 sheets A4 80gsm paper',          350.00, 100, '2024-03-15', 3),
        ('T-Shirt Basic',     'Cotton basic T-shirt',               499.00,  25, '2024-05-01', 4),
        ('Mineral Water 1L',  '1 litre mineral water bottle',        20.00, 500, '2024-05-20', 5),
        ('Coffee Beans 250g', 'Arabica coffee beans',               450.00,   7, '2024-04-10', 5);
    PRINT 'Seed products inserted.';
END
GO

-- Seed Vendors
IF NOT EXISTS (SELECT 1 FROM Vendors WHERE VendorName = 'TechSupply Co.')
BEGIN
    INSERT INTO Vendors (VendorName, Description, VendorEmail, Address, PhoneNumber,
                         CategoryID, ProductID, Quantity, PricePerUnit)
    VALUES
        ('TechSupply Co.',   'Electronics wholesale supplier',    'info@techsupply.com',  '123 Tech Park, Mumbai',      '+91-9876543210', 1, 1, 10, 72000.00),
        ('Office World',     'Office furniture and supplies',     'sales@officeworld.com', '45 Business Hub, Pune',     '+91-9123456789', 2, 4,  5, 11500.00),
        ('Paper Mart',       'Stationery and paper products',     'orders@papermart.com',  '67 Market Street, Delhi',   '+91-9988776655', 3, 6, 50,   140.00),
        ('Fashion Hub',      'Clothing and apparel supplier',     'buyer@fashionhub.com',  '89 Textile Zone, Surat',    '+91-9071234567', 4, 8, 20,   450.00),
        ('AquaPure Beverages','Beverage distributor',             'supply@aquapure.com',   '11 Industrial Area, Nashik', '+91-9000112233', 5, 9,100,    18.00);
    PRINT 'Seed vendors inserted.';
END
GO

PRINT 'Seed data script completed.';
GO

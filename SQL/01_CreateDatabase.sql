-- =============================================================
-- Script: 01_CreateDatabase.sql
-- Description: Creates the RegvedInventoryDB database
-- Run this script connected to the master database
-- =============================================================

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'RegvedInventoryDB')
BEGIN
    CREATE DATABASE RegvedInventoryDB;
    PRINT 'Database RegvedInventoryDB created successfully.';
END
ELSE
BEGIN
    PRINT 'Database RegvedInventoryDB already exists.';
END
GO

USE RegvedInventoryDB;
GO

# Setup & Deployment Guide

## Run these git commands to commit and push the production rewrite

Open a terminal (Command Prompt or PowerShell) in the repository root and run:

```powershell
# 1. Check git status
git status

# 2. Check git history for any accidentally committed secrets
git log --oneline

# 3. Stage all changes
git add .gitignore
git add appsettings.example.json
git add SETUP.md
git add README.md
git add RegvedInventoryDB.sln
git add "RegvedInventoryDB/"
git add "RegvedInventoryDB.Tests/"
git add "SQL/"

# 4. Verify no secrets are staged (check these files are NOT included)
git diff --cached --name-only

# 5. Commit
git commit -m "Production-grade rewrite: dashboard, DataTables, bug fixes, tests, SQL scripts

- Fix VendorController route bug (Details{id} -> Details/{id})
- Fix double conversion bug in InsertProductAsync
- Add ILogger to CategoryService and RecycleBinService
- Fix CustomExceptionFilter/ActionFilter/ResultFilter to use ILogger
- Remove debug TempData and console.log from ProductController views
- Add DashboardViewModel, IDashboardService, DashboardService
- Add GetDashboardStatsAsync to InventoryRepository
- Add CategoryName/ProductName to Vendor model and reads
- Upgrade layout to Bootstrap 5 with sidebar navigation
- Add DataTables (search/sort/pagination) to all list views
- Add low-stock badge alerts to Product list
- Add tabbed interface to RecycleBin view
- Rewrite all CRUD views with modern Bootstrap 5 cards
- Add SQL scripts: database, tables, stored procedures, seed data
- Add xUnit test project with model validation and controller tests
- Add .gitignore (blocks appsettings.Development.json, .env, etc.)
- Add appsettings.example.json as safe connection string template
- Deprecate ProductDAL.CS
- Update solution file to include test project
- Write professional README with Mermaid diagrams

Co-Authored-By: Claude Sonnet 4.6 <noreply@anthropic.com>"

# 6. Push
git push origin main
```

## After Pushing - Check for Leaked Secrets in Git History

If you ever accidentally committed secrets in a previous commit, run:

```powershell
# Search history for connection strings
git log -p | Select-String -Pattern "Server=.*Database="

# If found, use BFG Repo Cleaner or git filter-repo to purge them:
# https://rtyley.github.io/bfg-repo-cleaner/
```

## SQL Server Setup

Run scripts in this order:
```
SQL/01_CreateDatabase.sql
SQL/02_CreateTables.sql
SQL/03_StoredProcedures.sql
SQL/04_SeedData.sql   (optional dev seed data)
```

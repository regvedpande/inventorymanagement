using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RegvedInventoryDB.DAL;
using RegvedInventoryDB.Filters;
using RegvedInventoryDB.Services;

var builder = WebApplication.CreateBuilder(args);

// Register MVC services with a global exception filter.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(typeof(CustomExceptionFilter));
});

// Register the repository, services, and custom filters.
builder.Services.AddScoped<InventoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IVendorService, VendorService>();
builder.Services.AddScoped<IRecycleBinService, RecycleBinService>();
builder.Services.AddScoped<CustomActionFilter>();
builder.Services.AddScoped<CustomResultFilter>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Ensure the default route pattern includes the controller and action
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

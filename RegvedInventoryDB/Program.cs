using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RegvedInventoryDB.DAL;
using RegvedInventoryDB.Filters;
using RegvedInventoryDB.Services;

var builder = WebApplication.CreateBuilder(args);

// MVC with global exception filter
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<CustomExceptionFilter>();
});

// Antiforgery
builder.Services.AddAntiforgery();

// Repository and services
builder.Services.AddScoped<InventoryRepository>();
builder.Services.AddScoped<ICategoryService,   CategoryService>();
builder.Services.AddScoped<IProductService,    ProductService>();
builder.Services.AddScoped<IVendorService,     VendorService>();
builder.Services.AddScoped<IRecycleBinService, RecycleBinService>();
builder.Services.AddScoped<IDashboardService,  DashboardService>();

// Custom filters (DI-injectable)
builder.Services.AddScoped<CustomActionFilter>();
builder.Services.AddScoped<CustomResultFilter>();

// HTTP context accessor (for future middleware use)
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }));

app.Run();

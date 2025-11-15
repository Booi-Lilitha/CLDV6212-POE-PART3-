using Microsoft.EntityFrameworkCore;
using ABCRetailers.Data;
using ABCRetailers.Services;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------------
// 1. Add MVC
// ------------------------------------------------------------
builder.Services.AddControllersWithViews();

// ------------------------------------------------------------
// 2. Add Database
// ------------------------------------------------------------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ------------------------------------------------------------
// 3. Register Custom Services
// ------------------------------------------------------------
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddScoped<IFunctionsApi, FunctionsApiClient>();

// ------------------------------------------------------------
// 4. HttpClient for Azure Functions
// ------------------------------------------------------------
builder.Services.AddHttpClient("Functions", client =>
{
    client.BaseAddress = new Uri("https://localhost:7213/api/");
});

// ------------------------------------------------------------
// 5. Authentication + Authorization + Session
// ------------------------------------------------------------
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
    });

builder.Services.AddAuthorization();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

// ------------------------------------------------------------
// 6. Build the app
// ------------------------------------------------------------
var app = builder.Build();

// ------------------------------------------------------------
// 7. Middleware Pipeline
// ------------------------------------------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();          // ?? Before authentication
app.UseAuthentication();
app.UseAuthorization();

// ------------------------------------------------------------
// 8. Routes
// ------------------------------------------------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

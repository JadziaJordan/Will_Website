using Will_Website.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add HttpClient for API calls
builder.Services.AddHttpClient();

// Register the API service
builder.Services.AddScoped<ApiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Add specific routes for PayFast callbacks
app.MapControllerRoute(
    name: "payfast_return",
    pattern: "donate/return",
    defaults: new { controller = "Donate", action = "Return" });

app.MapControllerRoute(
    name: "payfast_cancel",
    pattern: "donate/cancel",
    defaults: new { controller = "Donate", action = "Cancel" });

app.MapControllerRoute(
    name: "payfast_notify",
    pattern: "donate/notify",
    defaults: new { controller = "Donate", action = "Notify" });

app.Run();
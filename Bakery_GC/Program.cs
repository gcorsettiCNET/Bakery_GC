using Bakery.Infrastructure.Extensions;
using Bakery.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ======= PRESENTATION LAYER SERVICES =======
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// ======= APPLICATION LAYER =======
builder.Services.AddApplicationLayer();

// ======= INFRASTRUCTURE LAYER =======
builder.Services.AddInfrastructure(builder.Configuration);

// ======= LOGGING =======
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Set logging levels
if (builder.Environment.IsDevelopment())
{
    builder.Logging.SetMinimumLevel(LogLevel.Debug);
}
else
{
    builder.Logging.SetMinimumLevel(LogLevel.Information);
}

var app = builder.Build();

// ======= DATABASE INITIALIZATION =======
// Initialize database with migrations and seed data
await app.Services.InitializeDatabaseAsync();

// ======= HTTP PIPELINE CONFIGURATION =======
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
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();

// ======= ROUTING =======
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();

// ======= APPLICATION STARTUP =======
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("🚀 Bakery application started successfully!");
logger.LogInformation("🏗️ Clean Architecture implemented with Repository Pattern + Unit of Work");
logger.LogInformation("📊 Database initialized with seed data");

app.Run();
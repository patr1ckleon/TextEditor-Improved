using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using TextEditor.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var useSqlite = builder.Configuration.GetValue<bool>("USE_SQLITE");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (useSqlite)
    {
        var sqlite = builder.Configuration.GetConnectionString("SqliteConnection") ?? "Data Source=app.db";
        options.UseSqlite(sqlite);
    }
    else
    {
        var sqlServer = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        options.UseSqlServer(sqlServer);
    }
});
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Ensure MVC views and Razor Pages hot-recompile during development
var mvcBuilder = builder.Services.AddControllersWithViews();
var pagesBuilder = builder.Services.AddRazorPages();
#if DEBUG
mvcBuilder.AddRazorRuntimeCompilation();
pagesBuilder.AddRazorRuntimeCompilation();
#endif

var app = builder.Build();

// Apply migrations automatically at startup (safe for demo/single instance)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // Apply EF Core migrations on startup (works for both SQL Server and SQLite)
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Support reverse proxies (e.g., Vercel/Azure frontends)
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Docs}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

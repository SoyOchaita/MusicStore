using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;          
using MusicStore.Models;
using System.IO;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Detecta si corre dentro de contenedor
var inContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

// Carpeta de datos: bajo /app (en Docker) o en el ContentRoot local
var dataDir = Path.Combine(builder.Environment.ContentRootPath, "App_Data");
Directory.CreateDirectory(dataDir);    // asegúrate que exista

var dbPath = Path.Combine(dataDir, "musicstore.db");
Console.WriteLine($"[DB PATH] {dbPath}");

// Cadena robusta (crea si no existe, cache compartida)
var csb = new SqliteConnectionStringBuilder
{
    DataSource = dbPath,
    Mode = SqliteOpenMode.ReadWriteCreate,
    Cache = SqliteCacheMode.Shared
};

builder.Services.AddDbContext<MusicStoreContext>(options =>
    options.UseSqlite(csb.ToString()));

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();

var app = builder.Build();

// Aplica migraciones + seed con pequeño retry por si hay carrera en hot reload
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<MusicStoreContext>();

    for (var attempt = 1; ; attempt++)
    {
        try
        {
            db.Database.Migrate();
            SeedData.Initialize(services);
            break;
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == 10 && attempt < 5) // disk I/O
        {
            await Task.Delay(400); // breve espera y reintenta
        }
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


// Cultura por defecto
var gt = new CultureInfo("es-GT");
var loc = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(gt),
    SupportedCultures = new[] { gt },
    SupportedUICultures = new[] { gt }
};




app.UseRequestLocalization(loc);
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
app.Run();

using Microsoft.EntityFrameworkCore;
using MusicStore;
using MusicStore.Data;
using MusicStore.Models;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// DbContexts
builder.Services.AddDbContext<MusicStoreContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity (UI + Roles)
builder.Services
    .AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// MVC + Razor Pages (para Identity UI)
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Sesión
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    // options.IdleTimeout = TimeSpan.FromMinutes(20);
});

var app = builder.Build();

// Migraciones y seed
using (var scope = app.Services.CreateScope())
{
    var sp = scope.ServiceProvider;

    // Aplica migraciones
    sp.GetRequiredService<ApplicationDbContext>().Database.Migrate();
    sp.GetRequiredService<MusicStoreContext>().Database.Migrate();

    // Seed de dominio (álbumes/genres/artists)
    SeedData.Initialize(sp);

    // Seed de roles/usuarios
    await IdentitySeeder.SeedAsync(sp);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Localización opcional
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

app.UseAuthentication();
app.UseAuthorization();

// Session ANTES del mapeo de rutas
app.UseSession();

// Rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

// Identity UI
app.MapRazorPages();

app.Run();

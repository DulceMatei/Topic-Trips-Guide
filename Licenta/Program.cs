using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Licenta.Areas.Identity.Data;
using Licenta.Data.Seeding;
using Licenta.Services;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Security.Principal;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("LicentaDbContextConnection") ?? throw new InvalidOperationException("Connection string 'LicentaDbContextConnection' not found.");

builder.Services.AddDbContext<LicentaDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("LicentaDbContextConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("LicentaDbContextConnection"))
    ));

// Serviciile de localizare
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("ro-RO"),
        new CultureInfo("en-US")
    };
    options.DefaultRequestCulture = new RequestCulture("ro-RO");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

// Configureaza Identity cu localizare
builder.Services.AddDefaultIdentity<LicentaUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()  // Adauga suport pentru roluri
.AddEntityFrameworkStores<LicentaDbContext>()
.AddErrorDescriber<RomanianIdentityErrorDescriber>(); // 

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages()
    .AddRazorPagesOptions(options =>
    {
        options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
    });
builder.Services.AddHttpClient();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
{
    opt.TokenLifespan = TimeSpan.FromHours(1);
});

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireUppercase = false;
    options.User.AllowedUserNameCharacters = null;
    options.User.RequireUniqueEmail = true;
});

var app = builder.Build();

app.UseRequestLocalization();

// Rolurile și adminul
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await CreateRolesAndAdminAsync(services);

    var context = services.GetRequiredService<LicentaDbContext>();

    // Seed-uri pentru toate traseele
    SeedVladTepes.Seed(context);
    SeedNapoleon.Seed(context);
    SeedOttoVonBismarck.Seed(context);
    SeedLisabonaCulinar.Seed(context);
    SeedLyonCulinar.Seed(context);
    SeedBolognaCulinar.Seed(context);
}

var blockedPaths = new[] {
    "/Identity/Account/Manage/TwoFactorAuthentication",
    "/Identity/Account/Manage/ExternalLogins",
    "/Identity/Account/Manage/GenerateRecoveryCodes",
    "/Identity/Account/Manage/Disable2fa",
    "/Identity/Account/Manage/EnableAuthenticator",
    "/Identity/Account/Manage/ResetAuthenticator",
    "/Identity/Account/Lockout",
    "/Identity/Account/ResendEmailConfirmation",
    "/Identity/Account/LoginWith2fa",
    "/Identity/Account/ExternalLogin",
    "/Identity/Account/LoginWithRecoveryCode"
};

app.Use(async (context, next) =>
{
    if (blockedPaths.Any(path => context.Request.Path.StartsWithSegments(path, StringComparison.OrdinalIgnoreCase)))
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("Not Found");
        return;
    }
    await next();
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

// Functia pentru crearea rolurilor și a adminului
async Task CreateRolesAndAdminAsync(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<LicentaUser>>();

    string[] roleNames = { "Admin", "User" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    //  Admin
    var adminEmail = "admin@test.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        var newUser = new LicentaUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            Nume = "Administrator",
            DataInregistrării = DateTime.Now
        };
        var result = await userManager.CreateAsync(newUser, "Admin123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newUser, "Admin");
        }
    }

    //  System
    var systemEmail = "system@topictrips.com";
    var systemUser = await userManager.FindByEmailAsync(systemEmail);
    if (systemUser == null)
    {
        var newSystemUser = new LicentaUser
        {
            UserName = systemEmail,
            Email = systemEmail,
            Nume = "System",
            DataInregistrării = DateTime.Now
        };
        var result = await userManager.CreateAsync(newSystemUser, "System123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newSystemUser, "Admin"); 
        }
    }
}
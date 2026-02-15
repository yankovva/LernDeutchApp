using LerningApp.Common;
using LerningApp.Data;
using LerningApp.Data.Models;
using LerningApp.Data.Repository;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Web.Infrastructure.Extensions;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;

using ApplicationUser = LerningApp.Data.Models.ApplicationUser;
using LerningAppContext = LerningApp.Data.LerningAppContext;
using NoOpEmailSender = LerningApp.Web.Infrastructure.NoOpEmailSender;

var builder = WebApplication.CreateBuilder(args);
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
// Add services to the container.
builder.Services
    .AddDbContext<LerningAppContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
    {
        ConfigureIdentity(options, builder);
    })
    .AddEntityFrameworkStores<LerningAppContext>()
    .AddDefaultTokenProviders();
    

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

builder.Services.RegisterRepositories();

builder.Services.AddTransient<IEmailSender, NoOpEmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();

if (app.Environment.IsDevelopment())
{
    await DbSeeder.SeedAsync(app.Services);
}

await app.ApplyMigrations();

app.Run();

static void ConfigureIdentity(IdentityOptions options, WebApplicationBuilder builder)
{
    
    options.Password.RequiredLength = builder.Configuration.GetValue<int>("Identity:Password:RequiredLength");
    options.Password.RequireNonAlphanumeric = builder.Configuration.GetValue<bool>("Identity:Password:RequireNonAlphanumeric");
    options.Password.RequireDigit = builder.Configuration.GetValue<bool>("Identity:Password:RequireDigits");
    options.Password.RequireLowercase = builder.Configuration.GetValue<bool>("Identity:Password:RequireLowercase");
    options.Password.RequireUppercase = builder.Configuration.GetValue<bool>("Identity:Password:RequireUppercase");
    options.Password.RequiredUniqueChars = builder.Configuration.GetValue<int>("Identity:Password:RequiredUniqueChars");

    options.SignIn.RequireConfirmedEmail = builder.Configuration.GetValue<bool>("Identity:SignIn:RequireConfirmedEmail");
    options.SignIn.RequireConfirmedPhoneNumber = builder.Configuration.GetValue<bool>("Identity:SignIn:RequireConfirmedPhoneNumber");
    options.SignIn.RequireConfirmedAccount = builder.Configuration.GetValue<bool>("Identity:SignIn:RequireConfirmedAccount");
        
    options.User.RequireUniqueEmail = builder.Configuration.GetValue<bool>("Identity:User:RequireUniqueEmail");

}
using LerningApp.Data;

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
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.SignIn.RequireConfirmedEmail = false;
    })
    .AddEntityFrameworkStores<LerningAppContext>()
    .AddDefaultTokenProviders();
    
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

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
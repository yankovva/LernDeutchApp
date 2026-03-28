using LerningApp.Common;
using LerningApp.Data;
using LerningApp.Data.Models;
using LerningApp.Data.Repository;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.Infrastructure.Extensions;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Blazor;
using ApplicationUser = LerningApp.Data.Models.ApplicationUser;
using LerningAppContext = LerningApp.Data.LerningAppContext;
using NoOpEmailSender = LerningApp.Web.Infrastructure.NoOpEmailSender;

var builder = WebApplication.CreateBuilder(args);
//Add services to the container
builder.Services.AddApplicationDbContext(builder.Configuration);
builder.Services.AddApplicationIdentity(builder.Configuration);
builder.Services.AddFacebookAuth(builder.Configuration);

builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

builder.Services.RegisterRepositories();
builder.Services.RegisterUserDefinedServices(typeof(CourseService).Assembly);

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

var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".mp3"] = "audio/mpeg";
provider.Mappings[".m4a"] = "audio/mp4";
provider.Mappings[".wav"] = "audio/wav";
provider.Mappings[".ogg"] = "audio/ogg";

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider
});

app.MapControllerRoute(
    name: "Areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

await app.ApplyMigrations();

if (app.Environment.IsDevelopment())
{
    await DbSeeder.SeedAsync(app.Services);
}
await app.SeedRolesAndAdminAsync();

app.Run();
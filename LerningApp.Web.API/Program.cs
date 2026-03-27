using LerningApp.Services.Data;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddApplicationDbContext(builder.Configuration);
builder.Services.AddApplicationIdentity(builder.Configuration);

builder.Services.RegisterRepositories();
builder.Services.RegisterUserDefinedServices(typeof(CourseService).Assembly);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
        options.AddPolicy("WebClient", policy =>
        {
                policy.WithOrigins("https://localhost:7222")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{ 
        app.UseSwagger();
        app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("WebClient");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
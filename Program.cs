using Microsoft.AspNetCore.Identity;
using Scalar.AspNetCore;
using WhiteListing_Backend.Models;
using WhiteListing_Backend.Stores;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//add identity types
builder.Services.AddIdentity<ApplicationUser, ApplicationUserStore>();


//Add identity services.
builder.Services.AddTransient<IUserStore<ApplicationUser>>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

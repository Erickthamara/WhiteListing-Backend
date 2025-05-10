using DotNetEnv;
using Microsoft.AspNetCore.Identity;
using Scalar.AspNetCore;
using Supabase;
using WhiteListing_Backend.Identity;
using WhiteListing_Backend.Models;
using WhiteListing_Backend.Stores;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
/// Load environment variables from .env file
Env.Load();
//Add env variables
string supabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL") ?? throw new ArgumentNullException("Supabse URL missing!");
string supabaseKey = Environment.GetEnvironmentVariable("SUPABASE_KEY") ?? throw new ArgumentNullException("Supabse KEY missing!");

// Add services to the container.
builder.Services.AddScoped<Supabase.Client>(_ => new Supabase.Client(
    supabaseUrl,
    supabaseKey,
     new SupabaseOptions
     {
         AutoRefreshToken = true,
         AutoConnectRealtime = true,
     }
    ));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHttpClient();

//add identity types
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddUserManager<CustomUserManager>()
    .AddDefaultTokenProviders();


// add identity services
builder.Services.AddScoped<IUserStore<ApplicationUser>, ApplicationUserStore>();
builder.Services.AddScoped<IRoleStore<ApplicationRole>, ApplicationRoleStore>();
//builder.Services.AddTransient<IUserPasswordStore<ApplicationUser>, ApplicationUserStore>();




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

using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Supabase;
using System.Text;
using TodoApi.Services;
using WhiteListing_Backend.Identity;
using WhiteListing_Backend.Models;
using WhiteListing_Backend.Stores;
using WhiteListing_Backend.SupabaseModels;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
/// Load environment variables from .env file
Env.Load();


// ...

//// Add this line where Env.Load() is called
//DotEnv.Load();
//DotEnv.Load();

//Add env variables
string supabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL") ?? throw new ArgumentNullException("Supabse URL missing!");
string supabaseKey = Environment.GetEnvironmentVariable("SUPABASE_KEY") ?? throw new ArgumentNullException("Supabse KEY missing!");
string JWT_Issuer = Environment.GetEnvironmentVariable("JWT_Issuer") ?? throw new ArgumentNullException("JWT Issuer missing!");
string JWT_Token = Environment.GetEnvironmentVariable("JWT_Token") ?? throw new ArgumentNullException("JWT Token missing!");
string JWT_Audience = Environment.GetEnvironmentVariable("JWT_Audience") ?? throw new ArgumentNullException("JWT Audience missing!");

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
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    //options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
})
    .AddUserManager<CustomUserManager>()
    .AddDefaultTokenProviders();


// add identity services
builder.Services.AddScoped<IUserStore<ApplicationUser>, ApplicationUserStore>();
builder.Services.AddScoped<IRoleStore<ApplicationRole>, ApplicationRoleStore>();
//builder.Services.AddTransient<IUserPasswordStore<ApplicationUser>, ApplicationUserStore>();



builder.Services.AddAuthentication(
    options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    //options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}
)
    //    .AddCookie(options =>
    //{
    //    options.Cookie.Name = "jwt_token"; // Name of the cookie to store JWT
    //})

    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = JWT_Issuer,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidAudience = JWT_Audience,
            // The default for this is 5 minutes, so if you're using short-lived tokens, set this to zero
            ClockSkew = TimeSpan.Zero, // <-- IMPORTANT
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWT_Token!)),
            ValidateIssuerSigningKey = true,
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var jwtCookie = context.Request.Cookies["jwt_token"];
                if (!string.IsNullOrEmpty(jwtCookie))
                {
                    context.Token = jwtCookie;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddScoped<IJWTAuthservice, JWTAuthService>();

// Add CORS policy
var myAllowedOrigins = "_WhitelistingApp";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowedOrigins,
       policy => policy.WithOrigins("https://au-whitelisting-demo.erickthamara.com", "http://localhost:5116", "https://localhost:5173/")
       .WithMethods("PUT", "DELETE", "GET", "POST")
       .AllowAnyHeader()
       .AllowCredentials());
    //.WithHeaders("Content-Type", "application/json")); // Allow required headers);
});



var app = builder.Build();

//Test Supabase connection
using (var scope = app.Services.CreateScope())
{
    try
    {
        var supabase = scope.ServiceProvider.GetRequiredService<Supabase.Client>();

        // Actually test the connection by making a simple query
        await supabase.From<SupabaseUserModel>().Get();

        app.Logger.LogInformation("✓ Supabase connection verified successfully");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "✗ Failed to connect to Supabase");
        //throw;
    }
}


//Handle preflight manually in case proxy strips headers
app.Use(async (context, next) =>
{
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.Headers.Append("Access-Control-Allow-Origin", "https://au-whitelisting-demo.erickthamara.com");
        context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
        context.Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type, Authorization");
        context.Response.Headers.Append("Access-Control-Allow-Credentials", "true");
        context.Response.StatusCode = 204;
        return;
    }
    await next();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}


//app.UseHttpsRedirection();

app.UseCors(myAllowedOrigins);
app.UseAuthentication();

app.UseAuthorization();

//app.Use(async (context, next) =>
//{
//    Console.WriteLine($"Incoming: {context.Request.Method} {context.Request.Path}");
//    await next();
//});

app.MapControllers();

app.Run();

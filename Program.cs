using StackExchange.Redis;
using MongoDB.Driver;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true; // Adds headers showing API versions
});


// **Configure Redis connection** (Redis Cloud)
var redisConnectionString = builder.Configuration.GetSection("Redis:ConnectionString").Value;
builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
{
    try
    {
        return ConnectionMultiplexer.Connect(redisConnectionString);
    }
    catch (Exception ex)
    {
        // Log error using ILogger
        Console.WriteLine($"Redis connection error: {ex.Message}");
        throw;
    }
});

// **Configure MongoDB connection** (MongoDB Atlas)
var mongoConnectionString = builder.Configuration.GetSection("MongoDB:ConnectionString").Value;
builder.Services.AddSingleton<IMongoClient, MongoClient>(_ =>
{
    try
    {
        return new MongoClient(mongoConnectionString);
    }
    catch (Exception ex)
    {
        // Log error using ILogger
        Console.WriteLine($"MongoDB connection error: {ex.Message}");
        throw;
    }
});

// **Add Controllers and Swagger**
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter your token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// **Add Google Authentication**
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration.GetValue<string>("GoogleAuth:ClientId");
    options.ClientSecret = builder.Configuration.GetValue<string>("GoogleAuth:ClientSecret");
    options.SaveTokens = true;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy => policy
            .WithOrigins("https://doctorappointmentservice-a3e5g7caccg6fwbt.germanywestcentral-01.azurewebsites.net") // Your frontend URL
            .AllowAnyHeader()
            .AllowAnyMethod());
});


var app = builder.Build();

app.UseCors("AllowSpecificOrigin");

// Enable serving of static files from the wwwroot folder
app.UseStaticFiles();

// **Set up Middleware**
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// **Swagger UI in Development Environment**
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();

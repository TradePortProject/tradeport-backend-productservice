using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProductManagement.aws;
using ProductManagement.Data;
using ProductManagement.Logger;
using ProductManagement.Logger.interfaces;
using ProductManagement.Mappings;
using ProductManagement.Repositories;
using Serilog;
using Serilog.Events;
using Serilog.Filters;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAutoMapper(typeof(ProductAutoMapperProfiles));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//scretet manager
var regionName = builder.Configuration["Secrets:AwsRegion"] ?? "ap-southeast-1";
var region = RegionEndpoint.GetBySystemName(regionName);

var dbSecretName = builder.Configuration["Secrets:DbSecretName"]
    ?? throw new InvalidOperationException("Secrets:DbSecretName is missing");

var sm = new AmazonSecretsManagerClient(region);


// helper
async Task<string> GetSecretStringAsync(string name)
{
    var resp = await sm.GetSecretValueAsync(new GetSecretValueRequest { SecretId = name });
    return resp.SecretString ?? "";
}

// ---- DB secret ----
var dbJson = await GetSecretStringAsync(dbSecretName);

// parse out the connection string value
using var doc = JsonDocument.Parse(dbJson);
var connectionStringFromSecret = doc.RootElement
    .GetProperty("ConnectionStrings")
    .GetProperty("tradeportdb")
    .GetString()!;

// add it into IConfiguration
builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    ["ConnectionStrings:tradeportdb"] = connectionStringFromSecret
});

// --- Use it with EF Core ---
var conn = builder.Configuration.GetConnectionString("tradeportdb");
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseSqlServer(conn, sql => sql.EnableRetryOnFailure()));

// Register repository
builder.Services.AddScoped<IProductRepository, ProductRepository>();

//AWS S3 Configuration
builder.Services.Configure<AwsOptions>(builder.Configuration.GetSection("AWS"));
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonS3>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
    builder =>
    {
        builder.WithOrigins("http://tradeport.cloud:3001")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});


builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(3015); 
    options.ListenAnyIP(3016);
    options.ListenAnyIP(3075);
    //options.ListenAnyIP(443, listenOptions => listenOptions.UseHttps()); // HTTPS port
});

builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
// Configure Serilog from appsettings.json
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)  
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Filter.ByIncludingOnly(Matching.FromSource("OrderManagement.Controllers.OrderManagementController"))
    .CreateLogger();
builder.Host.UseSerilog(); 
// Register IAppLogger<T>
builder.Services.AddScoped(typeof(IAppLogger<>), typeof(AppLogger<>));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        //options.RequireHttpsMetadata = false; // Set to true in production
        //options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"]            
        };

        Console.WriteLine($"Issuer: {options.TokenValidationParameters.ValidIssuer}");
        
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                Console.WriteLine($"Authorization header: {authHeader}");
                return Task.CompletedTask;
            },

            OnChallenge = context =>
            {
                context.HandleResponse(); 
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync("{\"message\": \"Token is missing or invalid\"}");
            },

            OnAuthenticationFailed = context =>
            {
                // Log authentication failure
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            }
        };
    });


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Management API v1");
    c.RoutePrefix = "swagger"; // Access at http://localhost:3016/swagger
});

// Create the folder if it doesn't exist
//string uploadPath = "/mnt/volume_sgp1_01/uploads/images";
//if (!Directory.Exists(uploadPath))
//{
//	Directory.CreateDirectory(uploadPath);
//}

//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
//        Path.Combine(builder.Environment.ContentRootPath, "wwwroot/uploads/images")),
//    RequestPath = "/uploads/images"
//});


// Enable CORS with the specified policy
app.UseCors("AllowSpecificOrigins");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

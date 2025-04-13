using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using ProductManagement.Mappings;
using ProductManagement.Repositories;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Text;
using ProductManagement.Logger.interfaces;
using ProductManagement.Logger;
using Serilog.Events;
using Serilog.Filters;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAutoMapper(typeof(ProductAutoMapperProfiles));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddDbContext<AppDbContext>(options =>
    //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//var connectionString = Environment.GetEnvironmentVariable("DEFAULT_CONNECTION") ??
                       //builder.Configuration.GetConnectionString("DefaultConnection");

// Add DbContext service
//builder.Services.AddDbContext<AppDbContext>(options =>
    //options.UseSqlServer(connectionString));

// Configure SqlClient to ignore certificate validation errors (for testing purposes)
SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder(builder.Configuration.GetConnectionString("DefaultConnection"));

sqlBuilder.Encrypt = true;
sqlBuilder.TrustServerCertificate = true;
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(sqlBuilder.ConnectionString));

// Register repository
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Add CORS service
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(3015); // HTTP port
    options.ListenAnyIP(3016);
    //options.ListenAnyIP(443, listenOptions => listenOptions.UseHttps()); // HTTPS port
});

builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
// Configure Serilog from appsettings.json
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)  // Reads from appsettings.json
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Filter.ByIncludingOnly(Matching.FromSource("OrderManagement.Controllers.OrderManagementController"))
    .CreateLogger();
builder.Host.UseSerilog(); // Register Serilog
// Register IAppLogger<T>
builder.Services.AddScoped(typeof(IAppLogger<>), typeof(AppLogger<>));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
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

        // Log the issuer for debugging purposes
        Console.WriteLine($"Issuer: {options.TokenValidationParameters.ValidIssuer}");

        // This is needed for CORS support
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
                context.HandleResponse(); // Prevent default unauthorized response
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

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

// Enable Swagger for all environments
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Management API v1");
    c.RoutePrefix = "swagger"; // Access at http://localhost:3016/swagger
});

// Create the folder if it doesn't exist
string uploadPath = "wwwroot/uploads/images";
if (!Directory.Exists(uploadPath))
{
	Directory.CreateDirectory(uploadPath);
}
	
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot/uploads/images")),
    RequestPath = "/uploads/images"
});

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// Enable CORS with the specified policy
app.UseCors("AllowAll");

app.Run();

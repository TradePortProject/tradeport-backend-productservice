using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using ProductManagement.Mappings;
using ProductManagement.Repositories;
using AutoMapper;
using System.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAutoMapper(typeof(ProductAutoMapperProfiles));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext service
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure SqlClient to ignore certificate validation errors (for testing purposes)
SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder(builder.Configuration.GetConnectionString("DefaultConnection"));

sqlBuilder.Encrypt = true;
sqlBuilder.TrustServerCertificate = true;
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(sqlBuilder.ConnectionString));
	

// Register repository
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
    builder =>
    {
        builder.WithOrigins("http://localhost:3001") // Add the ReactJS app origin
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(3015); // HTTP port
    options.ListenAnyIP(3016); 
    //options.ListenAnyIP(443, listenOptions => listenOptions.UseHttps()); // HTTPS port
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable CORS with the specified policy
app.UseCors("AllowSpecificOrigins");

app.UseAuthorization();

app.MapControllers();

app.Run();

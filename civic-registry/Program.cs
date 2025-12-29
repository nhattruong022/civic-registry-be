using Microsoft.EntityFrameworkCore;
using CivicRegistry.API.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Civic Registry API",
        Version = "v1",
        Description = "API documentation for Civic Registry Backend",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Civic Registry Team",
            Email = "support@civicregistry.com"
        }
    });

    // Include XML comments
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Entity Framework Core
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=CivicRegistryDB;Integrated Security=True;MultipleActiveResultSets=True;TrustServerCertificate=True";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Civic Registry API v1");
        c.RoutePrefix = "api-docs";
        c.DocumentTitle = "Civic Registry API Documentation";
    });
}

// app.UseHttpsRedirection(); // Disable HTTPS redirect for local development

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();

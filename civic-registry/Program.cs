using MongoDB.Driver;
using CivicRegistry.API.Models;
using CivicRegistry.API.Services;
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

// MongoDB Configuration
var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDB")
    ?? "mongodb://vpadmin:Vinpet2025%40@54.255.111.176:27017/admin";
var databaseName = builder.Configuration["MongoDB:DatabaseName"] ?? "CivicRegistryDB";

// Đăng ký MongoDB Client và Context
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    return new MongoClient(mongoConnectionString);
});

builder.Services.AddScoped<MongoDbContext>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return new MongoDbContext(client, databaseName);
});

// Đăng ký MongoIndexService
builder.Services.AddScoped<MongoIndexService>();

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

// Tạo indexes khi ứng dụng khởi động
using (var scope = app.Services.CreateScope())
{
    var indexService = scope.ServiceProvider.GetRequiredService<MongoIndexService>();
    await indexService.CreateIndexesAsync();
}

app.MapControllers();

app.Run();

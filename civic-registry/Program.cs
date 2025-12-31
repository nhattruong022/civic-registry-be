using MongoDB.Driver;
using CivicRegistry.API.Models;
using CivicRegistry.API.Services;
using CivicRegistry.API.Middleware;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

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

    // Cấu hình JWT Authentication cho Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Nhập token (Swagger sẽ tự động thêm 'Bearer ' prefix). Example: \"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
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

// Đăng ký AuthService
builder.Services.AddScoped<AuthService>();

// Đăng ký UserService
builder.Services.AddScoped<UserService>();

// Đăng ký RequestService
builder.Services.AddScoped<RequestService>();

// Đăng ký StatisticsService
builder.Services.AddScoped<StatisticsService>();

// Đăng ký HouseholdService
builder.Services.AddScoped<HouseholdService>();

// Đăng ký SeedDataService
builder.Services.AddScoped<SeedDataService>();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyForJWTTokenGeneration123456789";
var issuer = jwtSettings["Issuer"] ?? "CivicRegistryAPI";
var audience = jwtSettings["Audience"] ?? "CivicRegistryClient";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

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
// Swagger luôn hiển thị (không chỉ Development)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Civic Registry API v1");
    c.RoutePrefix = "api-docs";
    c.DocumentTitle = "Civic Registry API Documentation";
});

// app.UseHttpsRedirection(); // Disable HTTPS redirect for local development

app.UseCors("AllowAll");

// JWT Authentication Middleware
app.UseJwtAuthentication();

app.UseAuthentication();
app.UseAuthorization();

// Tạo indexes và seed data khi ứng dụng khởi động
using (var scope = app.Services.CreateScope())
{
    var indexService = scope.ServiceProvider.GetRequiredService<MongoIndexService>();
    await indexService.CreateIndexesAsync();

    // Seed dữ liệu mẫu
    var seedDataService = scope.ServiceProvider.GetRequiredService<SeedDataService>();
    await seedDataService.SeedAllAsync();
}

app.MapControllers();

app.Run();

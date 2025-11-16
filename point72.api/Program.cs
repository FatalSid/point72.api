using Microsoft.EntityFrameworkCore;
using point72.api.Data;
using point72.api.Repositories;
using point72.api.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure Entity Framework Core with In-Memory Database
// This eliminates the need for SQL Server setup and is perfect for development and demonstration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    // Use in-memory database - data persists during application lifetime
    options.UseInMemoryDatabase("Point72InversionDb");

    // Enable detailed errors in development
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Register application services following Dependency Injection best practices
builder.Services.AddScoped<IInversionRepository, InversionRepository>();
builder.Services.AddScoped<IWordInversionService, WordInversionService>();

// Configure API documentation with Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Point72 Word Inversion API",
        Version = "v1",
        Description = "REST API service for inverting words in sentences with persistent storage (in-memory database)",
        Contact = new()
        {
            Name = "Point72 Assessment",
            Email = "candidate@example.com"
        }
    });

    // Include XML comments for API documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Configure CORS if needed for frontend integration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add health checks for production monitoring
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

var app = builder.Build();

// Ensure the in-memory database is created
// No migrations needed for in-memory database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        logger.LogInformation("Ensuring in-memory database is created...");
        context.Database.EnsureCreated();
        logger.LogInformation("In-memory database is ready");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while initializing the database");
        throw;
    }
}

// Configure the HTTP request pipeline
// Enable Swagger in all environments for demonstration purposes
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Point72 Word Inversion API v1");
    c.RoutePrefix = "swagger"; // Serve Swagger UI at /swagger
});

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

// Map health check endpoint
app.MapHealthChecks("/health");

app.Run();

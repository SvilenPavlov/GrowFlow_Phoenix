using GrowFlow_Phoenix;
using GrowFlow_Phoenix.Data;
using GrowFlow_Phoenix.Infrastructure.Leviathan.Services;
using GrowFlow_Phoenix.Infrastructure.Phoenix.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GrowFlow_Phoenix API",
        Version = "v1",
        Description = "CRUD API for Employees with Leviathan synchronization"
    });
});

// EF Core SQLite
builder.Services.AddDbContext<PhoenixDbContext>(options =>
    options.UseSqlite("Data Source=phoenix.db"));

// Dependency injection
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddHostedService<LeviathanSyncService>();
builder.Services.AddHttpClient<LeviathanClient>();

builder.Services.AddAutoMapper(c=>c.AddProfile(typeof(MappingConfig)));

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PhoenixDbContext>();
    db.Database.EnsureCreated(); // Creates phoenix.db if it doesn't exist
}

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "GrowFlow_Phoenix API V1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

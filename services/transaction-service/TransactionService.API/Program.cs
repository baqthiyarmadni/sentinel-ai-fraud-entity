using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.EntityFrameworkCore;
using TransactionService.Application.Interfaces;
using TransactionService.API.Middlewares;
using TransactionService.Infrastructure.Database;
using TransactionService.Infrastructure.Repositories; // Verified Namespace
using TransactionService.Application.Mappings;
using TransactionService.Application.Repositories; // Verified Namespace
using AppServices = TransactionService.Application.Services; // Aliased to prevent namespace loops
using AutoMapper;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithProcessId()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// 1. Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Transaction Service API",
        Version = "v1",
        Description = "Enterprise Transaction Microservice"
    });
});

// 2. Configure PostgreSQL Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// 3. Register AutoMapper
builder.Services.AddAutoMapper(typeof(TransactionProfile));

// 4. Register Application Services (Dependency Injection)
// Using the explicit 'AppServices' alias eliminates the namespace loops completely
builder.Services.AddScoped<ITransactionService, AppServices.TransactionService>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

var app = builder.Build();

// 5. Global Exception Handling Middleware (Kept right at the start of pipeline execution)
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();


// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Safely redirect HTTP to HTTPS if ports are configured locally
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
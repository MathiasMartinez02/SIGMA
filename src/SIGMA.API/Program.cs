using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using SIGMA.API.Extensions;
using SIGMA.API.Middleware;
using SIGMA.Application;
using SIGMA.Infrastructure;
using SIGMA.Infrastructure.Persistence;
using SIGMA.Infrastructure.Persistence.Seed;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "SIGMA")
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File("logs/sigma-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSigmaSwagger();
builder.Services.AddSigmaAuthorization();

var corsOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:3000"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("SigmaFrontend", policy =>
    {
        policy.WithOrigins(corsOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Migrate and seed
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Initializing database...");
        var created = await context.Database.EnsureCreatedAsync();
        logger.LogInformation("Database EnsureCreated result: {Created} (true = tables created now, false = already existed)", created);
        await DataSeeder.SeedAsync(context, logger);
        logger.LogInformation("Database ready.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "FATAL: Database initialization failed: {Message}", ex.Message);
    }
}

// Middleware pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SIGMA API v1");
        c.RoutePrefix = "swagger";
    });
}

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseCors("SigmaFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

try
{
    Log.Information("Starting SIGMA API on port 8000...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "SIGMA API terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}

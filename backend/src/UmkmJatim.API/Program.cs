using Serilog;
using UmkmJatim.API.Extensions;
using UmkmJatim.API.Middleware;
using UmkmJatim.Infrastructure.Persistence.Seeders;

var builder = WebApplication.CreateBuilder(args);

// 1. Serilog Setup
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// 2. Services Registration
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerDocumentation();
builder.Services.AddCustomRateLimiting();

// Global Exception Handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// CORS
var allowedOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>() 
    ?? ["http://localhost:5173", "http://localhost:3000"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// 3. Auto Migration & Database Seeder in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pasar UMKM Jatim API v1");
        c.RoutePrefix = "swagger";
    });

    try
    {
        await DatabaseSeeder.SeedAsync(app.Services);
    }
    catch (Exception ex)
    {
        Log.Warning(ex, "Database seeding dilewati atau gagal.");
    }
}

// 4. Middleware Pipeline
app.UseExceptionHandler();

app.UseSerilogRequestLogging();

app.UseStaticFiles(); // Serve static uploads

app.UseRouting();

app.UseCors("AllowClientApp");

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

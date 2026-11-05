using System.Text;
using System.Threading.RateLimiting;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using UmkmJatim.Application.Common.Behaviors;
using UmkmJatim.Application.Common.Interfaces;
using UmkmJatim.Application.Mappings;
using UmkmJatim.Domain.Entities;
using UmkmJatim.Infrastructure.BackgroundJobs;
using UmkmJatim.Infrastructure.ExternalServices.Midtrans;
using UmkmJatim.Infrastructure.ExternalServices.Shipping;
using UmkmJatim.Infrastructure.FileStorage;
using UmkmJatim.Infrastructure.Identity;
using UmkmJatim.Infrastructure.Persistence;

namespace UmkmJatim.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var applicationAssembly = typeof(IApplicationDbContext).Assembly;

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(applicationAssembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(applicationAssembly);
        services.AddApplicationMappings();

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Settings options
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.Configure<FileStorageSettings>(configuration.GetSection(FileStorageSettings.SectionName));
        services.Configure<MidtransSettings>(configuration.GetSection(MidtransSettings.SectionName));

        // DbContext
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? "Server=localhost,1433;Database=UmkmJatimDb;User Id=sa;Password=YourStrongP@ssw0rd!;TrustServerCertificate=True;";

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString, b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        // Identity
        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 8;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // Services
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ICurrentUser, CurrentUserService>();
        services.AddScoped<IFileStorage, LocalFileStorageService>();
        services.AddScoped<IShippingService, MockShippingService>();

        // Background Job
        services.AddHostedService<OrderExpiryService>();

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() ?? new JwtSettings();
        var key = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(jwtSettings.Secret) ? "default-super-secret-key-32-chars-long!" : jwtSettings.Secret);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
            options.AddPolicy("RequireSellerRole", policy => policy.RequireRole("Seller"));
            options.AddPolicy("RequireCustomerRole", policy => policy.RequireRole("Customer"));
        });

        return services;
    }

    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Pasar UMKM Jawa Timur API",
                Version = "v1",
                Description = "RESTful API untuk Marketplace UMKM Jawa Timur dengan autentikasi JWT + refresh token, RBAC, dan pembayaran online Midtrans."
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header menggunakan skema Bearer. Masukkan 'Bearer [token]'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }

    public static IServiceCollection AddCustomRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // Strict policy untuk login & register (anti brute force)
            options.AddFixedWindowLimiter("auth", opt =>
            {
                opt.PermitLimit = 5;
                opt.Window = TimeSpan.FromMinutes(1);
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 0;
            });

            // Policy untuk checkout / order creation
            options.AddFixedWindowLimiter("checkout", opt =>
            {
                opt.PermitLimit = 10;
                opt.Window = TimeSpan.FromMinutes(1);
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 0;
            });
        });

        return services;
    }
}

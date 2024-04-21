using System.Text;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Services.Logging;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Realizations.Base;
using Streetcode.BLL.Interfaces.Email;
using Streetcode.BLL.Services.Email;
using Streetcode.DAL.Entities.AdditionalContent.Email;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Services.BlobStorageService;
using Streetcode.BLL.Interfaces.Users;
using Microsoft.FeatureManagement;
using Streetcode.BLL.Interfaces.Payment;
using Streetcode.BLL.Services.Payment;
using Streetcode.BLL.Interfaces.Instagram;
using Streetcode.BLL.Services.Instagram;
using Streetcode.BLL.Interfaces.Text;
using Streetcode.BLL.Services.Text;
using FluentValidation.AspNetCore;
using FluentValidation;
using Streetcode.BLL.ActionFilters;
using Streetcode.BLL.MediatR.Streetcode.Fact.Create;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Entities.AdditionalContent.Jwt;
using Streetcode.BLL.Services.Users;
using Microsoft.Extensions.Options;

namespace Streetcode.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddRepositoryServices(this IServiceCollection services)
    {
        services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
        services.AddScoped<IEntityRepositoryBase<Fact>, EntityRepositoryBase<Fact>>();
    }

    public static void AddCustomServices(this IServiceCollection services, IHostEnvironment hostEnvironment)
    {
        services.AddRepositoryServices();
        services.AddFeatureManagement();
        var currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        services.AddAutoMapper(currentAssemblies);
        services.AddMediatR(currentAssemblies);

        services.AddScoped<IBlobService>(sp =>
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Local";
            var logger = sp.GetRequiredService<ILoggerService>();
            var repo = sp.GetRequiredService<IRepositoryWrapper>();
            if (environment == "IntegrationTests" || environment == "Local")
            {
                var options = sp.GetRequiredService<IOptions<BlobEnvironmentVariables>>();
                return new BlobService(options, logger, repo);
            }

            var configuration = sp.GetRequiredService<IConfiguration>();
            return new AzureBlobService(configuration, logger, repo);
        });
        services.AddScoped<ILoggerService, LoggerService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IInstagramService, InstagramService>();
        services.AddScoped<ITextService, AddTermsToTextService>();
        services.AddScoped<ITokenService, TokenService>();
    }

    public static void AddApplicationServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        string connectionString;
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Local";
        if (environment == "IntegrationTests" || environment == "Local")
        {
            var connection = configuration.GetSection(environment).GetConnectionString("DefaultConnection");
            connectionString = connection ?? throw new InvalidOperationException($"'DefaultConnection' is null or not found for the '{environment}' environment.");
        }
        else
        {
            var connection = configuration.GetConnectionString("DefaultConnection");
            connectionString = connection ?? throw new InvalidOperationException("'DefaultConnection' is null or not found");
        }

        var emailConfig = configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();

        if (emailConfig is not null)
        {
            services.AddSingleton(emailConfig);
        }

        var jwtConfig = configuration.GetSection("Jwt").Get<JwtConfiguration>();

        if (jwtConfig is not null)
        {
            services.AddSingleton(jwtConfig);
        }

        var refreshTokenConfig = configuration.GetSection("RefreshToken").Get<RefreshTokenConfiguration>();

        if (refreshTokenConfig is not null)
        {
            services.AddSingleton(refreshTokenConfig);
        }

        services.AddDbContext<StreetcodeDbContext>(options =>
        {
            options.UseSqlServer(connectionString, opt =>
            {
                opt.MigrationsAssembly(typeof(StreetcodeDbContext).Assembly.GetName().Name);
                opt.MigrationsHistoryTable("__EFMigrationsHistory", schema: "entity_framework");
            });
        });

        services.AddHangfire(config =>
        {
            config.UseSqlServerStorage(connectionString);
        });

        services.AddHangfireServer();

        var corsConfig = configuration.GetSection("CORS").Get<CorsConfiguration>();
        services.AddCors(opt =>
        {
            opt.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        services.AddHsts(opt =>
        {
            opt.Preload = true;
            opt.IncludeSubDomains = true;
            opt.MaxAge = TimeSpan.FromDays(30);
        });

        services.AddScoped<ModelStateFilter>();
        services.AddScoped<AsyncValidateEntityExistsFilter<Fact>>();
        services.AddLogging();
        services.AddControllers(options =>
        {
            options.Filters.Add<ModelStateFilter>();
        });
        services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssemblyContaining<CreateFactDtoValidator>();
    }

    public static void AddSwaggerServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyApi", Version = "v1" });

            opt.CustomSchemaIds(x => x.FullName);

            opt.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = JwtBearerDefaults.AuthenticationScheme
            });

            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme,
                        },
                        Scheme = JwtBearerDefaults.AuthenticationScheme,
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
        });
    }

    public class CorsConfiguration
    {
        public List<string> AllowedOrigins { get; set; }
        public List<string> AllowedHeaders { get; set; }
        public List<string> AllowedMethods { get; set; }
        public int PreflightMaxAge { get; set; }
    }
}

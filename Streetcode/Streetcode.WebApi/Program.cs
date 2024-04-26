using Hangfire;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.Services.BlobStorageService;
using Streetcode.DAL.Entities.Users;
using Streetcode.WebApi.Extensions;
using Streetcode.WebApi.Middlewares;
using Streetcode.WebApi.Utils;
using Streetcode.DAL.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureApplication();

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddSwaggerServices();
builder.Services.AddCustomServices();
builder.Services.ConfigureBlob(builder);
builder.Services.ConfigurePayment(builder);
builder.Services.ConfigureInstagram(builder);
builder.Services.ConfigureSerilog(builder);

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
 .AddEntityFrameworkStores<StreetcodeDbContext>()
 .AddUserStore<UserStore<ApplicationUser, ApplicationRole, StreetcodeDbContext, int>>()
 .AddRoleStore<RoleStore<ApplicationRole, StreetcodeDbContext, int>>()
 .AddDefaultTokenProviders();

builder.Services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

// JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Jwt").GetValue<string>("Key")!))
        };
    });

builder.Services.AddAuthorization(options =>
{
});

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.EnvironmentName == "Local")
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPIv5 v1"));
}
else
{
    app.UseHsts();
}

await app.ApplyMigrations();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.SeedRoles().Wait();
    scope.ServiceProvider.SeedAdmin(builder).Wait();
}

await app.SeedDataAsync(); // uncomment for seeding data in local
app.UseCors();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard("/dash");

if (app.Environment.EnvironmentName != "Local")
{
    BackgroundJob.Schedule<WebParsingUtils>(
    wp => wp.ParseZipFileFromWebAsync(), TimeSpan.FromMinutes(1));
    RecurringJob.AddOrUpdate<WebParsingUtils>(
        wp => wp.ParseZipFileFromWebAsync(), Cron.Monthly);
    RecurringJob.AddOrUpdate<AzureBlobService>(
        b => b.CleanBlobStorageAsync(default), Cron.Monthly);
    BackgroundJob.Schedule<DeleteExpiredRefreshTokensUtils>(
        dt => dt.DeleteExpiredRefreshTokens(), TimeSpan.FromDays(10));
}

app.MapControllers();

app.Run();
public partial class Program
{
}

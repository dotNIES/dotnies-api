using CommunityToolkit.Mvvm.Messaging;
using dotNIES.API.Core.Repositories;
using dotNIES.API.Core.Repositories.Common;
using dotNIES.API.Core.Repositories.Finance;
using dotNIES.API.Core.Repositories.Internal;
using dotNIES.API.Helpers;
using dotNIES.Data.Dto.Internal;
using dotNIES.Data.Logging.Messages;
using dotNIES.Data.Logging.Models;
using dotNIES.Data.Logging.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Diagnostics.Metrics;
using System.Text;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

RegisterDI(builder.Services);
ConfigureServices(builder.Services);

var connectionString = builder.Configuration["TestConnectionString"]; //.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();
InitializeBaseObjects(app.Services);
ConfigureMiddleware(app);

app.Run();


void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = ".NIES API", Version = "v1" });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
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
                new string[] {}
            }
        });
    });

    services.AddHealthChecks();

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
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
}

void RegisterDI(IServiceCollection services)
{
    var config = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                            .Build();

    // Internal things
    services.AddSingleton<IConfiguration>(config);
    services.AddSingleton<IUserAppInfoDto, UserAppInfoDto>();
    services.AddSingleton<IAppInfoDto, AppInfoDto>();

    // dotNIES.Data.Logging Services
    services.AddSingleton<ILoggerService, LoggerService>();

    // dotNIES.API.Core repositories
    services.AddSingleton<IBaseRepository, BaseRepository>();
    services.AddScoped<ICategoryRepository, CategoryRepository>();
    services.AddScoped<IGeneralLedgerDetailRepository, GeneralLedgerDetailRepository>();
    services.AddScoped<IGeneralLedgerRepository, GeneralLedgerRepository>();
    services.AddScoped<IPaymentTypeRepository, PaymentTypeRepository>();
    services.AddScoped<IPurchaseTypeRepository, PurchaseTypeRepository>();
    services.AddScoped<IVendorRepository, VendorRepository>();
}

void InitializeBaseObjects(IServiceProvider serviceProvider)
{
    var config = serviceProvider.GetRequiredService<IConfiguration>();
    var loggerService = serviceProvider.GetRequiredService<ILoggerService>();
    var userAppInfoDto = serviceProvider.GetRequiredService<IAppInfoDto>();
    var appInfoDto = serviceProvider.GetRequiredService<IUserAppInfoDto>();

    // Check development or production
    if (!config.GetSection("IsDevelopment").Exists())
    {
        var logMessage = new LogMessageModel
        {
            Message = "The parameter IsDevelopment in the appsettings.json was not set.",
            LogLevel = LogLevel.Error
        };
        WeakReferenceMessenger.Default.Send(new LogMessage(logMessage));

        throw new Exception("The parameter IsDevelopment in the appsettings.json was not set.");
    }
    else
    {
        appInfoDto.IsDevelopment = config.GetSection("IsDevelopment").Value?.ToLower() == "true";
    }

    // Get the minimum loglevel (default = Information)
    if (!config.GetSection("MinimumLogLevel").Exists())
    {
        var logMessage = new LogMessageModel
        {
            Message = "The parameter MinimumLogLevel in the appsettings.json was not set.",
            LogLevel = LogLevel.Error
        };
        WeakReferenceMessenger.Default.Send(new LogMessage(logMessage));
        throw new Exception("The parameter MinimumLogLevel in the appsettings.json was not set.");
    }
    else
    {
        var minLogLevel = config.GetSection("MinimumLogLevel").Value ?? string.Empty;

        appInfoDto.MinimumLogLevel = minLogLevel switch
        {
            "Trace" => LogLevel.Trace,
            "Debug" => LogLevel.Debug,
            "Information" => LogLevel.Information,
            "Warning" => LogLevel.Warning,
            "Error" => LogLevel.Error,
            "Critical" => LogLevel.Critical,
            _ => LogLevel.Information,
        };
    }

    // get info about logging the entire record or not
    if (!config.GetSection("LogEntireRecord").Exists())
    {
        var logMessage = new LogMessageModel
        {
            Message = "The parameter LogEntireRecord in the appsettings.json was not set.",
            LogLevel = LogLevel.Error
        };
        WeakReferenceMessenger.Default.Send(new LogMessage(logMessage));
        throw new Exception("The parameter LogEntireRecord in the appsettings.json was not set.");
    }
    else
    {
        appInfoDto.LogEntireRecord = config.GetSection("LogEntireRecord").Value?.ToLower() == "true";
    }

    // get info about logging the sql statements
    if (!config.GetSection("LogSqlStatements").Exists())
    {
        var logMessage = new LogMessageModel
        {
            Message = "The parameter LogSqlStatements in the appsettings.json was not set.",
            LogLevel = LogLevel.Error
        };
        WeakReferenceMessenger.Default.Send(new LogMessage(logMessage));
        throw new Exception("The parameter LogSqlStatements in the appsettings.json was not set.");
    }
    else
    {
        appInfoDto.LogSqlStatements = config.GetSection("LogSqlStatements").Value?.ToLower() == "true";
    }

    // fill in the rest of the application parameters
    appInfoDto.AppSessionId = Guid.NewGuid();
    appInfoDto.UserLoggerInfoId = Guid.NewGuid();
    appInfoDto.UserName = Environment.UserName;
    appInfoDto.WindowsUserName = Environment.UserName;

    // fill in the user application parameters
    userAppInfoDto.AppName = "dotNIES API";
    userAppInfoDto.AppVersion = "1.0.0";

    // Get the connectionstring
    if (appInfoDto.IsDevelopment)
    {
        userAppInfoDto.ConnectionString = config.GetSection("TestConnectionString").Value ?? string.Empty;

        var logMessage = new LogMessageModel
        {
            Message = "Using the Testconnectionstring.",
            LogLevel = LogLevel.Debug
        };
        WeakReferenceMessenger.Default.Send(new LogMessage(logMessage));
    }
    else
    {
        userAppInfoDto.ConnectionString = config.GetSection("ProdConnectionString").Value ?? string.Empty;

        var logMessage = new LogMessageModel
        {
            Message = "Using the Prodcutionconnectionstring.",
            LogLevel = LogLevel.Debug
        };
        WeakReferenceMessenger.Default.Send(new LogMessage(logMessage));
    }

    //var seeder = new IdentitySeeder(userAppInfoDto.ConnectionString);
    //seeder.SeedAccounts();
}

void ConfigureMiddleware(WebApplication app)
{
    // Poort instellen
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    app.Urls.Add($"http://0.0.0.0:{port}");

    app.UseStaticFiles();
    app.UseRouting();

    //app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapHealthChecks("/");
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", ".Nies API v1");
    });

    app.MapAreaControllerRoute(
        name: "areas",
        areaName: "common",
        pattern: "{area:exists}/{controller}/{action=Index}/{id?}");

    app.MapControllers();
}

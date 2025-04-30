using CommunityToolkit.Mvvm.Messaging;
using dotNIES.API.Areas.common.Controllers;
using dotNIES.API.Core.Repositories.Internal;
using dotNIES.API.Core.Services;
using dotNIES.Data.Dto.Internal;
using dotNIES.Data.Logging.Messages;
using dotNIES.Data.Logging.Models;
using dotNIES.Data.Logging.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder.Services);
RegisterDI(builder.Services);

var app = builder.Build();
ConfigureMiddleware(app);

InitializeBaseObjects(app.Services);

app.Run();


void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = ".NIES API", Version = "v1" });
    });

    services.AddHealthChecks();
}

void RegisterDI(IServiceCollection services)
{
    var config = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                            .Build();

    // Internal things
    services.AddSingleton(config);
    services.AddSingleton<IUserAppInfoDto, UserAppInfoDto>();
    services.AddSingleton<IAppInfoDto, AppInfoDto>();

    // dotNIES.Data.Logging Services
    services.AddSingleton<ILoggerService, LoggerService>();

    // dotNIES.API.Core services
    services.AddSingleton<IBaseRepository, BaseRepository>();
    services.AddScoped<ICategoryService, CategoryService>();
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
        appInfoDto.IsDevelopment = config.GetSection("IsDevelopment").Value == "true";
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
        appInfoDto.LogEntireRecord = config.GetSection("LogEntireRecord").Value == "true";
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
        appInfoDto.LogSqlStatements = config.GetSection("LogSqlStatements").Value == "true";
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
        userAppInfoDto.ConnectionString = config.GetConnectionString("TestConnectionString") ?? string.Empty;

        var logMessage = new LogMessageModel
        {
            Message = "Using the Testconnectionstring.",
            LogLevel = LogLevel.Debug
        };
        WeakReferenceMessenger.Default.Send(new LogMessage(logMessage));
    }
    else
    {
        userAppInfoDto.ConnectionString = config.GetConnectionString("ProdConnectionString") ?? string.Empty;

        var logMessage = new LogMessageModel
        {
            Message = "Using the Prodcutionconnectionstring.",
            LogLevel = LogLevel.Debug
        };
        WeakReferenceMessenger.Default.Send(new LogMessage(logMessage));
    }
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

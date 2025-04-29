using dotNIES.Data.Dto.Internal;
using dotNIES.Data.Logging.Models;
using dotNIES.Data.Logging.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ?? Setup services
ConfigureServices(builder.Services);

// ?? Setup app pipeline
var app = builder.Build();
ConfigureMiddleware(app);

// ?? Run app
app.Run();

// --------------------
//  Method Definitions
// --------------------

void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = ".NIES API", Version = "v1" });
    });

    services.AddHealthChecks();

    services.AddSingleton<IUserAppInfoDto, UserAppInfoDto>();
    services.AddSingleton<ILoggerService, LoggerService>();
}

void ConfigureMiddleware(WebApplication app)
{
    // Poort instellen
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    app.Urls.Add($"http://0.0.0.0:{port}");

    app.UseStaticFiles();
    app.UseRouting();

    //app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapHealthChecks("/");
    app.UseSwagger();
    app.UseSwaggerUI();

    app.MapAreaControllerRoute(
        name: "areas",
        areaName: null,
        pattern: "{area:exists}/{controller}/{action=Index}/{id?}");

    app.MapControllers();
}

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

// ------------------
// Method Definitions
// ------------------

void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v2", new OpenApiInfo { Title = "My API", Version = "v2" });
    });

    services.AddHealthChecks();

    services.AddSingleton<IUserLoggerInfoModel, UserLoggerInfoModel>();
    services.AddSingleton<ILoggerService, LoggerService>();
}

void ConfigureMiddleware(WebApplication app)
{
    // Poort instellen
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    app.Urls.Add($"http://0.0.0.0:{port}");

    app.UseStaticFiles();
    app.UseRouting();

    // https redirect eventueel uitzetten tijdens debuggen
    // app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapHealthChecks("/");
    app.MapControllers();

    app.UseSwagger();
    app.UseSwaggerUI();
}

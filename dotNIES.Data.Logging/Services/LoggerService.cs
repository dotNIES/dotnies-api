using CommunityToolkit.Mvvm.Messaging;
using dotNIES.Data.Logging.Helpers;
using dotNIES.Data.Logging.Messages;
using dotNIES.Data.Logging.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.MsSqlServer.Destructurers;
using Serilog.Sinks.SystemConsole.Themes;
using System.Diagnostics;

namespace dotNIES.Data.Logging.Services;

/// <summary>
/// This class is responsible for logging messages.
/// 
/// Use:    see the documentation for information about the use of this service
/// </summary>
public class LoggerService : ILoggerService
{
    private readonly IUserLoggerInfoModel _userLoggerInfoModel;

    public LoggerService(IUserLoggerInfoModel userLoggerInfoModel)
    {
        _userLoggerInfoModel = userLoggerInfoModel;

        InitializeSerilog();
        RegisterMessages();
    }

    private void InitializeSerilog()
    {
        var configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .Build();

        var enricher = new ClassNameMethodNameEnricher(_userLoggerInfoModel);

        // Set up Serilog configuration
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.With(enricher)
            .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                .WithDefaultDestructurers()
                .WithDestructurers(new[] { new SqlExceptionDestructurer() }))
            .CreateLogger();

        Log.Information("LOGGING STARTED");
    }

    private void RegisterMessages()
    {
        WeakReferenceMessenger.Default.Register<LogMessage>(this, (r, m) =>
        {
            if (m == null || m.Value == null)
            {
                Log.Error("The logmessage was null. Please do not log NULL messages!");
                return;
            }

            if (string.IsNullOrWhiteSpace(m.Value.Message))
            {
                Log.Error("The logmessage was empty. Please do not log empty messages!");
                return;
            }

            var logLevel = m.Value.LogLevel;

            switch (logLevel)
            {
                case LogLevel.Information:
                    Log.Information(m.Value.Message);
                    break;
                case LogLevel.Warning:
                    Log.Warning(m.Value.Message);
                    break;
                case LogLevel.Error:
                    Log.Error(m.Value.Exception, m.Value.Message);
                    break;
                case LogLevel.Debug:
                    Log.Debug(m.Value.Message);
                    break;
                case LogLevel.Critical:
                    Log.Fatal(m.Value.Exception, m.Value.Message);
                    break;
                default:
                    Log.Information("Default loglevel 'information' used.");
                    Log.Information(m.Value.Message);
                    break;
            }
        });

        WeakReferenceMessenger.Default.Register<LogRecordMessage>(this, (r, m) =>
        {
            if (m == null || m.Value == null)
            {
                Log.Error("The logmessage was null. Please do not log NULL messages!");
                return;
            }

            if (string.IsNullOrWhiteSpace(m.Value.Message))
            {
                Log.Error("The logmessage was empty. Please do not log empty messages!");
                return;
            }

            if (string.IsNullOrWhiteSpace(m.Value.Record))
            {
                Log.Error("The logrecord was null. Please do not log NULL records!");
                Log.Error($"The logmessage was: {m.Value.Message}");
                return;
            }

            var logLevel = m.Value.LogLevel;

            var logMessage = m.Value.Message;
            logMessage += "\n";
            logMessage += "----- RECORD -----";
            logMessage += "\n";
            logMessage += m.Value.Record;

            switch (logLevel)
            {
                case LogLevel.Information:
                    Log.Information(m.Value.Message);
                    break;
                case LogLevel.Warning:
                    Log.Warning(m.Value.Message);
                    break;
                case LogLevel.Error:
                    Log.Error(m.Value.Exception, m.Value.Message);
                    break;
                case LogLevel.Debug:
                    Log.Debug(m.Value.Message);
                    break;
                case LogLevel.Critical:
                    Log.Fatal(m.Value.Exception, m.Value.Message);
                    break;
                default:
                    Log.Information("Default loglevel 'information' used.");
                    Log.Information(m.Value.Message);
                    break;
            }
        });
    }

    public Guid GetUserLoggerId()
    {
        var result = _userLoggerInfoModel.UserLoggerInfoId;
        return result;
    }
}

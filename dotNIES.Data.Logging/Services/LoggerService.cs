using CommunityToolkit.Mvvm.Messaging;
using dotNIES.Data.Dto.Internal;
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
    private readonly IUserAppInfoDto _userLoggerInfoModel;

    public LoggerService(IUserAppInfoDto userLoggerInfoModel)
    {
        _userLoggerInfoModel = userLoggerInfoModel;

        InitializeSerilog();
        RegisterMessages();
    }

    [Obsolete("Do not use this method anymore", true)]
    public Guid GetUserLoggerId() => _userLoggerInfoModel.UserLoggerInfoId;

    /// <summary>
    /// Logs debug information to the configured sinks.
    /// </summary>
    /// <param name="message">The message to log</param>
    /// <param name="exception">Optional exception to include</param>
    /// <remarks>
    /// <para>
    /// WARNING: This is a synchronous method and may block the executing thread.
    /// Consider using <see cref="SendDebugInfoAsync"/> for better performance.
    /// </para>
    /// </remarks>
    public void SendDebugInfo(string message, Exception? exception = null) => Send(LogLevel.Information, message, exception);

    /// <summary>
    /// Logs information to the configured sinks.
    /// </summary>
    /// <param name="message">The message to log</param>
    /// <param name="exception">Optional exception to include</param>
    /// <remarks>
    /// <para>
    /// WARNING: This is a synchronous method and may block the executing thread.
    /// Consider using <see cref="SendInformationAsync"/> for better performance.
    /// </para>
    /// </remarks>
    public void SendInformation(string message, Exception? exception = null) => Send(LogLevel.Information, message, exception);

    /// <summary>
    /// Logs warning to the configured sinks.
    /// </summary>
    /// <param name="message">The message to log</param>
    /// <param name="exception">Optional exception to include</param>
    /// <remarks>
    /// <para>
    /// WARNING: This is a synchronous method and may block the executing thread.
    /// Consider using <see cref="SendWarningAsync"/> for better performance.
    /// </para>
    /// </remarks>
    public void SendWarning(string message, Exception? exception = null) => Send(LogLevel.Warning, message, exception);

    /// <summary>
    /// Logs error to the configured sinks.
    /// </summary>
    /// <param name="message">The message to log</param>
    /// <param name="exception">Optional exception to include</param>
    /// <remarks>
    /// <para>
    /// WARNING: This is a synchronous method and may block the executing thread.
    /// Consider using <see cref="SendErrorAsync"/> for better performance.
    /// </para>
    /// </remarks>
    public void SendError(string message, Exception? exception = null) => Send(LogLevel.Error, message, exception);

    public Task SendDebugInfoAsync(string message, Exception? exception = null) => SendAsync(LogLevel.Information, message, exception);

    public Task SendInformationAsync(string message, Exception? exception = null) => SendAsync(LogLevel.Information, message, exception);

    public Task SendWarningAsync(string message, Exception? exception = null) => SendAsync(LogLevel.Warning, message, exception);

    public Task SendErrorAsync(string message, Exception? exception = null) => SendAsync(LogLevel.Error, message, exception);

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

    private void Send(LogLevel level, string message, Exception? exception)
    {
        var logMessage = new LogMessageModel
        {
            LogLevel = level,
            Message = message,
            Exception = exception
        };

        WeakReferenceMessenger.Default.Send(logMessage);
    }

    /// <summary>
    /// Sends a log message asynchronously to the configured sinks.
    /// </summary>
    /// <param name="level">The log level</param>
    /// <param name="message">The message to log</param>
    /// <param name="exception">Optional exception to include</param>
    /// <returns>A Task representing the asynchronous operation</returns>
    private Task SendAsync(LogLevel level, string message, Exception? exception)
    {
        var logMessage = new LogMessageModel
        {
            LogLevel = level,
            Message = message,
            Exception = exception
        };
        return Task.Run(() => WeakReferenceMessenger.Default.Send(logMessage));
    }
}

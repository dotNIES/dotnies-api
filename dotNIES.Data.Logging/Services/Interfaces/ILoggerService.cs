
namespace dotNIES.Data.Logging.Services;

public interface ILoggerService
{
    [Obsolete("Do not use this method anymore", true)]
    Guid GetUserLoggerId();

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
    void SendDebugInfo(string message, Exception? exception = null);

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
    void SendInformation(string message, Exception? exception = null);

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
    void SendWarning(string message, Exception? exception = null);

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
    void SendError(string message, Exception? exception = null);

    // Asynchrone methoden
    Task SendDebugInfoAsync(string message, Exception? exception = null);
    Task SendInformationAsync(string message, Exception? exception = null);
    Task SendWarningAsync(string message, Exception? exception = null);
    Task SendErrorAsync(string message, Exception? exception = null);
}
using Microsoft.Extensions.Logging;

namespace dotNIES.Data.Logging.Models;

/// <summary>
/// This class is used to store the log message.
/// </summary>
public class LogMessageModel
{
    public LogLevel LogLevel { get; set; }
    public string? Message { get; set; }
    public Exception? Exception { get; set; }
}

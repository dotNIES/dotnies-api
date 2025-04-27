using dotNIES.Data.Logging.Messages;
using dotNIES.Data.Logging.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

public static class LogRecordHelper
{
    public static LogRecordMessage CreateLogRecordMessage<T>(object record, LogLevel logLevel = LogLevel.Information, string? message = null, Exception? exception = null)
    {
        var model = new LogRecordMessageModel
        {
            LogLevel = logLevel,
            Message = message,
            Exception = exception,
            Record = JsonSerializer.Serialize(record)
        };

        return new LogRecordMessage(model);
    }
}
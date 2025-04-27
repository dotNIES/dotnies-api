using dotNIES.Data.Logging.Models;

/// <summary>
/// This class is used to store the log message and you can add a record to it.
/// </summary>
/// <remarks>
/// for logging the record is serialized to JSON.
/// </remarks>
/// <typeparam name="T"></typeparam>
public class LogRecordMessageModel : LogMessageModel
{
    public string? Record { get; set; }
}
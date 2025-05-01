
namespace dotNIES.Data.Logging.Services;

public interface ILoggerService
{
    Guid GetUserLoggerId();
    void SendDebugInfo(string message, Exception? exception = null);
    void SendError(string message, Exception? exception = null);
    void SendInformation(string message, Exception? exception = null);
    void SendWarning(string message, Exception? exception = null);
}
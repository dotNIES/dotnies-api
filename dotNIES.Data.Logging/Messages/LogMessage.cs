using CommunityToolkit.Mvvm.Messaging.Messages;
using dotNIES.Data.Logging.Models;

namespace dotNIES.Data.Logging.Messages;
public class LogMessage : ValueChangedMessage<LogMessageModel>
{
    public LogMessage(LogMessageModel value) : base(value)
    {
    }
}

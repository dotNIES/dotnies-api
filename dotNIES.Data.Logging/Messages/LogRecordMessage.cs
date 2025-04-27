using CommunityToolkit.Mvvm.Messaging.Messages;
using dotNIES.Data.Logging.Models;

namespace dotNIES.Data.Logging.Messages;
public class LogRecordMessage : ValueChangedMessage<LogRecordMessageModel>
{
    public LogRecordMessage(LogRecordMessageModel value) : base(value)
    {
    }
}

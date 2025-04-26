using dotNIES.Data.Logging.Models;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNIES.Data.Logging.Helpers;
public class ClassNameMethodNameEnricher : ILogEventEnricher
{
    private readonly IUserLoggerInfoModel _userLoggerInfoModel;

    public ClassNameMethodNameEnricher(IUserLoggerInfoModel userLoggerInfoModel)
    {
        _userLoggerInfoModel = userLoggerInfoModel;
    }

    /// <summary>
    /// Enrich the serilog log-entry with Class, Method en een Sessie-ID. 
    /// </summary>
    /// <param name="logEvent"></param>
    /// <param name="propertyFactory"></param>
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var frame = new StackFrame(13);
        var method = frame.GetMethod();

        if (method == null)
        {
            var className = "unknown class";
            var methodName = "unknown property";

            var classProperty = propertyFactory.CreateProperty("Class", className);
            var methodProperty = propertyFactory.CreateProperty("Method", methodName);
            logEvent.AddPropertyIfAbsent(classProperty);
            logEvent.AddPropertyIfAbsent(methodProperty);
        }
        else
        {
            var className = method.DeclaringType?.FullName ?? "unknown class";
            var methodName = method.Name;

            var classProperty = propertyFactory.CreateProperty("Class", className);
            var methodProperty = propertyFactory.CreateProperty("Method", methodName);
            logEvent.AddPropertyIfAbsent(classProperty);
            logEvent.AddPropertyIfAbsent(methodProperty);
        }

        var sessionId = _userLoggerInfoModel.AppSessionId == Guid.Empty ? Guid.NewGuid() : _userLoggerInfoModel.AppSessionId;
        var sessionProperty = propertyFactory.CreateProperty("SessionId", sessionId);
        logEvent.AddPropertyIfAbsent(sessionProperty);

        var userId = string.IsNullOrWhiteSpace(_userLoggerInfoModel.UserName) ? System.Security.Principal.WindowsIdentity.GetCurrent().Name : _userLoggerInfoModel.WindowsUserName;
        var userIdProperty = propertyFactory.CreateProperty("UserId", userId);
        logEvent.AddPropertyIfAbsent(userIdProperty);
    }
}
